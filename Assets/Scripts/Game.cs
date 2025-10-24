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
    public static readonly StatusEffect STATUS_Defend = Resources.Load<StatusEffect>("StatusEffects/Defend");
    public static readonly StatusEffect STATUS_ReduceDamage = Resources.Load<StatusEffect>("StatusEffects/ReduceDamage");
    
    public static readonly StatusEffect STATUS_Defense = Resources.Load<StatusEffect>("StatusEffects/Defense");
    public static readonly StatusEffect STATUS_Attack = Resources.Load<StatusEffect>("StatusEffects/Attack");

    public static readonly StatusEffect STATUS_ModDefense = Resources.Load<StatusEffect>("StatusEffects/ModDefense");
    public static readonly StatusEffect STATUS_ModAttack = Resources.Load<StatusEffect>("StatusEffects/ModAttack");

    public static StatusEffect StringToStatus(string statusName) {
        switch (statusName.ToLower()) {
            case "status_shatter":
                return STATUS_Shatter;
            case "status_phased":
                return STATUS_Phased;
            case "status_defend":
                return STATUS_Defend;
            case "status_reducedamage":
                return STATUS_ReduceDamage;
            case "status_defense":
                return STATUS_Defense;
            case "status_attack":
                return STATUS_Attack;
            case "status_moddefense":
                return STATUS_ModDefense;
            case "status_modattack":
                return STATUS_ModAttack;
            default:
                Debug.LogError("StatusEffect " + statusName + " does not exist!");
                return null;
        }
    }
}
