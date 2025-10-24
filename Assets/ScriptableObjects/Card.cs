using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effect {
    // Used to indicate the next effects are to the self
    Self,
    // Used to indicate the next effects are to the target
    Target,

    // Game effects
    DealDamage,
    GainMeteor,
    GainStatus,
    RecoverCard,
    Move,
    GainStat,
    UseMeteor,
    AddTargets,

    // Control flow effects
    SetFlag,
    IfFlags,
    Else,
    EndIfFlags,
}

public enum Rarity {
    Common,
    Uncommon,
    Rare,
    Special
}

public enum CardPhase {
    Pre,
    Main,
    Response,
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    [System.Serializable]
    public class CardEffect {
        public Effect effect;
        public string value;
    }

    [Header("Card Info")]
    public string cardName;
    [TextArea(3, 10)]
    public string description;
    public Sprite artwork;
    public Rarity rarity;

    [Header("Card Stats")]
    public CardPhase phase;
    public int apCost;

    [Header("Card Targeting")]
    public int range;
    // Usually a number of targets or "SELF" or "ALL"
    public string targets;

    [Header("Card Effects")]
    [TextArea(1, 10)]
    // A space-separated list of keywords that have special meaning
    // Usually used to identify a type of card, like a Phase card
    public string keywords;
    // This is typically for stuff like paying with meteors or adding additional targets
    // Nothing here should involve a choice and happens before targets are selected
    public List<CardEffect> preprocess;

    public List<CardEffect> effects;

    // Check for special keywords, like spending meteors or being a phase card
    // Keywords can have numbers at the end, for examp "SpendMeteor2"
    // So the beginning is treated as the actual keyword and the number as the value (if needed)
    public bool HasKeyword(string keyword) {
        var splits = keywords.Split(' ');
        foreach (var k in splits) {
            if (k.ToLower().StartsWith(keyword.ToLower()))
                return true;
        }
        return false;
    }

    public int GetKeywordValue(string keyword) {
        var splits = keywords.Split(' ');
        foreach (var k in splits) {
            if (k.ToLower().StartsWith(keyword.ToLower())) {
                var numberPart = k.Substring(keyword.Length);
                if (int.TryParse(numberPart, out int value))
                    return value;
                else
                    return 0; // keyword exists but has no number
            }
        }
        return -1; // keyword does not exist
    }
}
