using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Enemy Ability")]
public class EnemyAbility : ScriptableObject
{
    public int range;
    [Header("Card Effects")]
    public List<Card.CardEffect> effects;
}
