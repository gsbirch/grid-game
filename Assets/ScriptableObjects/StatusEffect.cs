using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status", menuName = "Status Effect")]
public class StatusEffect : ScriptableObject
{
    public string statusName;
    // make description a paragraph box in the inspector
    [TextArea(3, 10)]
    public string description;
    public Sprite icon;

    public int maxStacks = 99;

    public string ToIcon() {
        return $"<sprite name=\"{icon.name}\">";
    }
}
