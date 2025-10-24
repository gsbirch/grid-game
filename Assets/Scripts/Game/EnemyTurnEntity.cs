using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// This class holds all the "Game" information about the enemy
public class EnemyTurnEntity : BaseTargettable, ITurnEntity
{
    public Enemy Enemy;

    public SelectArrow selectArrow;

    Dictionary<string, int> stats = new();
    public event Action<string, int, int> OnStatChanged;

    public Dictionary<StatusEffect, int> statuses = new();

    public TextMeshPro statusText;

    private void Start() {
        InitiativeManager.Instance.RegisterEntity(this);

        stats.Add(GameManager.HEALTH_STAT, Enemy.health);

        OnStatChanged += (stat, oldValue, newValue) => {
            if (stat == GameManager.HEALTH_STAT && newValue <= 0) {
                // Enemy is dead
                Debug.Log("Enemy " + Enemy.enemyName + " is dead!");
                InitiativeManager.Instance.UnregisterEntity(this);
                Destroy(gameObject);
            }
            HUDBehaviour.Instance.UpdateUI();
        };
    }


    public void EndOfTurn() {
        return;
    }

    public Sprite GetInitiativePicture() {
        return Enemy.initiativePicture;
    }

    public void StartOfTurn() {
        GameManager.Instance.GameState = GameState.EnemyTurn;
        return;
    }

    public void TakeTurn() {
        var ability = PickRandomAbility();
        ProcessEffects(ability);

        // For now, the enemy just immediately passes its turn
        // after a short delay
        this.Invoke(() => {
            if (GameManager.Instance.GameState != GameState.EnemyTurn) return;
            ContinueTurn();
        }, 0.5f);
    }

    public void ContinueTurn() {
        // Since we are allowed to interupt the enemies turn, they need to be able to continue it
        InitiativeManager.Instance.PassTurn();
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

    public void ProcessEffects(EnemyAbility a) {
        // Card effects will change who is the target of the following effects
        bool targetIsSelf = true;
        var effects = a.effects;

        // Targets are all assumed to be enemies for the time being
        // If an effect would affect an enemy, it is assumed to affect all targets
        for (int i = 0; i < effects.Count; i++) {
            var playerPos = Player.Instance.GetComponent<TileEntity>().CellPosition;
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
                    if (targetIsSelf)
                        Debug.LogWarning("Attempting to deal damage to self, no cards do so (yet!)");
                    else {
                        var playerEntity = Player.Instance.GetComponent<ITargettable>();
                        if (Utilities.Distance(playerPos, GetComponent<TileEntity>().CellPosition) <= a.range) {
                            playerEntity.TakeDamage(amt, this, true);
                        }
                    }
                    break;
                case Effect.GainMeteor:
                    Debug.LogError("Error: Enemies can not gain meteors");
                    break;
                case Effect.GainStatus:
                    break;
                case Effect.RecoverCard:
                    Debug.LogError("Error: Enemies can not recover cards");
                    break;
                case Effect.Move:
                    var path = TilemapManager.Instance.FindPath(GetComponent<TileEntity>().CellPosition, playerPos);
                    int pathIdx = 0;
                    var movementLeft = int.Parse(effect.value);
                    if (path != null && path.Count > 1) {
                        while (movementLeft > 0 && pathIdx < path.Count - 1 && Utilities.Distance(path[pathIdx], playerPos) > a.range) {
                            pathIdx++;
                            movementLeft--;
                        }
                    }
                    if (pathIdx > 0) {
                        GetComponent<TileEntity>().UpdateCellPosition(path[pathIdx]);
                    }
                        

                    break;
                case Effect.GainStat:
                    break;
            }
        }
    }

    public override void SetSelectionState(SelectArrowState state) {
        selectArrow.SetState(state);
    }

    public EnemyAbility PickRandomAbility() {
        return Enemy.abilities[UnityEngine.Random.Range(0, Enemy.abilities.Count)];
    }

    public override void ApplyStatus(StatusEffect status, int count) {
        base.ApplyStatus(status, count);
        UpdateStatusesText();
    }

    public override void RemoveStatus(StatusEffect status, int count) {
        base.RemoveStatus(status, count);
        UpdateStatusesText();
    }

    void UpdateStatusesText() {
        string text = "";
        foreach (var status in statuses) {
            text += status.Key.ToIcon() + (status.Value > 1 ? status.Value : "");
        }
        statusText.text = text;
    }

    public override void TakeDamage(int amount, ITargettable source, bool procs) {
        base.TakeDamage(amount, source, procs);
    }
}
