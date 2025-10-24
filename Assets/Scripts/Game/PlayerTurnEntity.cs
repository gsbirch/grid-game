using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class holds all the "Game" information about the player
// Such as their stats, statuses, cards, etc.
// It also respects the ITurnEntity interface so it can take turns in the initiative order
public class PlayerTurnEntity : BaseTargettable, ITurnEntity
{

    // singleton
    public static PlayerTurnEntity Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public Sprite initiativePicture;

    public List<Card> deck = new();
    public List<Card> hand = new();
    public List<Card> discard = new();

    Dictionary<string, int> stats = new();
    public event Action<string, int, int> OnStatChanged;

    int bonusTargets = 0;

    public HashSet<string> flags = new();

    // Start is called before the first frame update
    void Start()
    {
        stats.Add(GameManager.HEALTH_STAT, 20);
        stats.Add(GameManager.AP_STAT, 3);
        stats.Add(GameManager.REFRESH_STAT, 1);
        stats.Add(GameManager.MOVEMENT_STAT, 2);
        stats.Add(GameManager.METEOR_STAT, 0);

        foreach (var card in deck) {
            hand.Add(card);
        }

        HUDBehaviour.Instance.ShowCardList(CardPhase.Main);

        InitiativeManager.Instance.RegisterEntity(this);

        HUDBehaviour.Instance.UpdateUI();

        GameManager.Instance.OnGameStateChanged += (previousState, newState) => {
            if (newState == GameState.PlayerTurnDefault) {
                HUDBehaviour.Instance.ShowCardList(CardPhase.Main);
                HUDBehaviour.Instance.ShowTurnUI();
            } else {
                HUDBehaviour.Instance.HideCardList();
                HUDBehaviour.Instance.HideTurnUI();
            }
        };
    }

    public override int GetStat(string stat) {
        if (!stats.ContainsKey(stat)) {
            Debug.LogError("Stat " + stat + " does not exist!");
            return -1;
        }
        return stats[stat];
    }

    public override void ModifyStat(string stat, int amount) {
        if (!stats.ContainsKey(stat)) {
            Debug.LogError("Stat " + stat + " does not exist!");
            return;
        }

        var oldValue = stats[stat];
        stats[stat] += amount;
        if (stats[stat] < 0) stats[stat] = 0;

        OnStatChanged?.Invoke(stat, oldValue, stats[stat]);
    }

    public void TakeTurn() {
        GameManager.Instance.GameState = GameState.PlayerTurnDefault;
        return;
    }

    public void StartOfTurn() {
        RemoveStatus(Game.STATUS_Phased, 1);
        RemoveStatus(Game.STATUS_Defend, 1);

        // Update ATK/DEF based on MOD ATK/DEF
        if (GetStacksOfStatus(Game.STATUS_ModAttack) != 0) {
            if (GetStacksOfStatus(Game.STATUS_ModAttack) > 0) {
                ApplyStatus(Game.STATUS_Attack, GetStacksOfStatus(Game.STATUS_ModAttack));
                RemoveStatus(Game.STATUS_ModAttack, GetStacksOfStatus(Game.STATUS_ModAttack));
            }
            else {
                RemoveStatus(Game.STATUS_Attack, -GetStacksOfStatus(Game.STATUS_ModAttack));
                ApplyStatus(Game.STATUS_ModAttack, -GetStacksOfStatus(Game.STATUS_ModAttack));
            }
        }
        if (GetStacksOfStatus(Game.STATUS_ModDefense) != 0) {
            if (GetStacksOfStatus(Game.STATUS_ModDefense) > 0) {
                ApplyStatus(Game.STATUS_Defense, GetStacksOfStatus(Game.STATUS_ModDefense));
                RemoveStatus(Game.STATUS_ModDefense, GetStacksOfStatus(Game.STATUS_ModDefense));
            }
            else {
                RemoveStatus(Game.STATUS_Defense, -GetStacksOfStatus(Game.STATUS_ModDefense));
                ApplyStatus(Game.STATUS_ModDefense, -GetStacksOfStatus(Game.STATUS_ModDefense));
            }
        }
        
        stats[GameManager.AP_STAT] = 3;
        stats[GameManager.MOVEMENT_STAT] = 2;
    }

    public void EndOfTurn() {
        return;
    }

    public Sprite GetInitiativePicture() {
        return initiativePicture;
    }

    public void CardClick(Card c, int meteorsToSpend) {
        RemoveFlag(Game.FLAG_SpentMeteor);
        bonusTargets = 0;
        //Debug.Log($"Playing {c.cardName}");
        if (stats[GameManager.AP_STAT] < c.apCost) {
            HUDBehaviour.Instance.ShowNotification("Not enough AP!", Color.red);
            return;
        }
        if (c.HasKeyword("Phase") && HasStatus(Game.STATUS_Phased)) {
            HUDBehaviour.Instance.ShowNotification("You can not play any more Phase cards this turn!", Color.red);
            return;
        }
        if (stats[GameManager.METEOR_STAT] < meteorsToSpend) {
            HUDBehaviour.Instance.ShowNotification("Not enough Meteors!", Color.red);
            return;
        }
        if (meteorsToSpend > 0 && !c.HasKeyword("SpendMeteor")) {
            HUDBehaviour.Instance.ShowNotification("This card does not benefit from spending Meteors!", Color.red);
            return;
        }
        if (meteorsToSpend > 0) {
            AddFlag(Game.FLAG_SpentMeteor);
        }
        PreProcessEffects(c.preprocess);
        if (GameManager.Instance.GameState == GameState.PlayerTurnResponse)
            ResponseManager.Instance.SelectCard(c);
        else
            TargetManager.Instance.InitalizeTargetSelection(GetComponent<TileEntity>().CellPosition, c, bonusTargets);
    }

    public void PlayCard(Card p, List<ITargettable> targets) {
        
        hand.Remove(p);
        discard.Add(p);
        bonusTargets = 0;
        ProcessEffects(p.effects, targets);
        ModifyStat(GameManager.AP_STAT, -p.apCost);

        if (HasFlag(Game.FLAG_SpentMeteor)) {
            RemoveFlag(Game.FLAG_SpentMeteor);
            ModifyStat(GameManager.METEOR_STAT, p.GetKeywordValue(Game.KEYWORD_SpendMeteor));
        }

        if (GameManager.Instance.GameState != GameState.PlayerTurnResponse &&
            GameManager.Instance.GameState != GameState.PlayerTurnCardRecovery)
            GameManager.Instance.GameState = GameState.PlayerTurnDefault;
    }
    
    public void PreProcessEffects(List<Card.CardEffect> effects) {
        bool takeElse = false;
        for (int i = 0; i < effects.Count; i++) {
            var effect = effects[i];
            switch (effect.effect) {
                case Effect.IfFlags:
                    var flags = effect.value.ToLower().Split(' ');
                    bool hasAllFlags = true;
                    foreach (var flag in flags) {
                        if (!HasFlag(flag)) {
                            hasAllFlags = false;
                            break;
                        }
                    }
                    if (!hasAllFlags) {
                        // skip to else or endif
                        while (i < effects.Count && effects[i].effect != Effect.Else && effects[i].effect != Effect.EndIfFlags) {
                            i++;
                            if (i < effects.Count && effects[i].effect == Effect.IfFlags) {
                                Debug.LogError("Nested IF statements are not supported");
                                return;
                            }
                        }
                        if (i < effects.Count && effects[i].effect == Effect.Else) {
                            takeElse = true;
                        }

                    }
                    break;
                case Effect.Else:
                    if (!takeElse) {
                        // skip to endif
                        while (i < effects.Count && effects[i].effect != Effect.EndIfFlags) {
                            i++;
                            if (i < effects.Count && effects[i].effect == Effect.IfFlags) {
                                Debug.LogError("Nested IF statements are not supported");
                                return;
                            }
                        }
                    }
                    break;
                case Effect.EndIfFlags:
                    takeElse = false;
                    break;
                case Effect.AddTargets:
                    bonusTargets += int.Parse(effect.value);
                    break;
                default:
                    Debug.LogError("Invalid effect in PreProcessEffects: " + effect.effect);
                    break;
            }
        }
    }

    // Process a list of effects
    // This will probably be the most complex function in the game
    public void ProcessEffects(List<Card.CardEffect> effects, List<ITargettable> targets) {
        // Card effects will change who is the target of the following effects
        bool targetIsSelf = true;
        bool takeElse = false;

        // Targets are all assumed to be enemies for the time being
        // If an effect would affect an enemy, it is assumed to affect all targets
        for (int i = 0; i < effects.Count; i++) {
            var effect = effects[i];
            switch (effect.effect) {
                case Effect.Self:
                    targetIsSelf = true;
                    break;
                case Effect.Target:
                    targetIsSelf = false;
                    break;
                case Effect.DealDamage:
                    int amt = int.Parse(effect.value);
                    amt += GetStacksOfStatus(Game.STATUS_Attack);
                    if (targetIsSelf)
                        Debug.LogWarning("Attempting to deal damage to self, no cards do so (yet!)");
                    else {
                        for (int j = 0; j < targets.Count; j++) {
                            targets[j].TakeDamage(amt, this, true);
                        }
                    }
                    break;
                case Effect.GainMeteor:
                    if (!targetIsSelf)
                        Debug.LogWarning("GainMeteor effect applied to target instead of self!");
                    else {
                        int meteorAmt = int.Parse(effect.value);
                        if (GetStat(GameManager.METEOR_STAT) + meteorAmt > Game.METEOR_MAX) {
                            ModifyStat(GameManager.METEOR_STAT, Game.METEOR_MAX - GetStat(GameManager.METEOR_STAT));
                            HUDBehaviour.Instance.ShowNotification("Meteors at maximum amount!", Color.yellow);
                        } else {
                            ModifyStat(GameManager.METEOR_STAT, meteorAmt);
                        }
                    }
                    break;
                case Effect.GainStatus:
                    // split effect.value by space to get status name and count
                    var parts = effect.value.Split(' ');
                    var status = Game.StringToStatus(parts[0]);
                    var count = parts.Length > 1 ? int.Parse(parts[1]) : 1;
                    if (status == null) {
                        Debug.LogError("StatusEffect " + effect.value + " does not exist!");
                        break;
                    }
                    if (targetIsSelf)
                        ApplyStatus(status, count);
                    else {
                        for (int j = 0; j < targets.Count; j++) {
                            targets[j].ApplyStatus(status, count);
                        }
                    }
                    break;
                case Effect.RecoverCard:
                    if (!targetIsSelf) {
                        Debug.LogWarning("RecoverCard effect applied to target instead of self!");
                    }
                    CardRecoverManager.Instance.InitalizeCardRecovery(int.Parse(effect.value));
                    break;
                case Effect.Move:
                    break;
                case Effect.GainStat:
                    if (!targetIsSelf) {
                        Debug.LogWarning("GainStat effect applied to target instead of self! This might be okay");
                        for (int j = 0; j < targets.Count; j++)
                            targets[j].ModifyStat(effect.value.Split(' ')[0], int.Parse(effect.value.Split(' ')[1]));
                    }
                    else
                        ModifyStat(effect.value.Split(' ')[0], int.Parse(effect.value.Split(' ')[1]));
                    break;
                
                case Effect.IfFlags:
                    var flags = effect.value.Split(' ');
                    bool hasAllFlags = true;
                    foreach (var flag in flags) {
                        if (!HasFlag(flag)) {
                            hasAllFlags = false;
                            break;
                        }
                    }
                    if (!hasAllFlags) {
                        // skip to else or endif
                        while (i < effects.Count && effects[i].effect != Effect.Else && effects[i].effect != Effect.EndIfFlags) {
                            i++;
                            if (i < effects.Count && effects[i].effect == Effect.IfFlags) {
                                Debug.LogError("Nested IF statements are not supported");
                                return;
                            }
                        }
                        if (i < effects.Count && effects[i].effect == Effect.Else) {
                            takeElse = true;
                        }

                    }
                    break;
                case Effect.Else:
                    if (!takeElse) {
                        // skip to endif
                        while (i < effects.Count && effects[i].effect != Effect.EndIfFlags) {
                            i++;
                            if (i < effects.Count && effects[i].effect == Effect.IfFlags) {
                                Debug.LogError("Nested IF statements are not supported");
                                return;
                            }
                        }
                    }
                    break;
                case Effect.EndIfFlags:
                    takeElse = false;
                    break;
            }
        }
    }

    public override void SetSelectionState(SelectArrowState state) {
        Debug.LogError("PlayerTurnEntity does not have a selection state!");
    }

    public override void ApplyStatus(StatusEffect status, int count) {
        base.ApplyStatus(status, count);
        HUDBehaviour.Instance.UpdateUI();
    }

    public override void RemoveStatus(StatusEffect status, int count) {
        base.RemoveStatus(status, count);
        HUDBehaviour.Instance.UpdateUI();
    }

    public override void TakeDamage(int amount, ITargettable source, bool procs) {
        if (procs) {
            // check if the player has response cards left
            // we can skip responses if there's no cards
            for (int i = 0; i < hand.Count; i++) {
                if (hand[i].phase == CardPhase.Response) {
                    ResponseManager.Instance.InitializeResponseSelection(amount, source);
                    return;
                }
            }
        }

        base.TakeDamage(amount, source, procs);
    }

    public void ProcessDamage(int amount, ITargettable source, bool procs) {
        base.TakeDamage(amount, source, procs);
    }

    public void AddFlag(string flag) {
        flags.Add(flag.ToLower());
    }

    public void RemoveFlag(string flag) {
        flags.Remove(flag.ToLower());
    }

    public bool HasFlag(string flag) {
        return flags.Contains(flag.ToLower());
    }

    // Recover a card from the discard pile
    // There MUST be a copy in the discard pile
    public void RecoverCard(Card c) {
        if (!discard.Contains(c)) {
            Debug.LogError("Attempted to recover a card that is not in the discard pile: " + c.cardName);
            return;
        }
        discard.Remove(c);
        hand.Add(c);
        HUDBehaviour.Instance.UpdateUI();
    }
}