using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

// This is a simple database class to hold all of the statuses and other game-wide data
// Everything in it should be static
// The name "Game" is chosen to be as simple as possible
public class Game : MonoBehaviour
{
    // Keywords
    public static readonly string KEYWORD_Phase = "phase";
    public static readonly string KEYWORD_SpendMeteor = "spendmeteor";

    // Flags
    public static readonly string FLAG_SpentMeteor = "spentmeteor";

    // Game Constants
    public static readonly int METEOR_MAX = 5;

    // Status Effects
    public static readonly StatusEffect STATUS_Shatter = Resources.Load<StatusEffect>("StatusEffects/Shatter");
    public static readonly StatusEffect STATUS_Phased = Resources.Load<StatusEffect>("StatusEffects/Phased");
    public static readonly StatusEffect STATUS_BloodMoon = Resources.Load<StatusEffect>("StatusEffects/BloodMoon");
    public static readonly StatusEffect STATUS_Defend = Resources.Load<StatusEffect>("StatusEffects/Defend");

    public static StatusEffect StringToStatus(string statusName) {
        switch (statusName) {
            case "STATUS_Shatter":
                return STATUS_Shatter;
            case "STATUS_Phased":
                return STATUS_Phased;
            case "STATUS_BloodMoon":
                return STATUS_BloodMoon;
            case "STATUS_Defend":
                return STATUS_Defend;
            default:
                Debug.LogError("StatusEffect " + statusName + " does not exist!");
                return null;
        }
    }
}
