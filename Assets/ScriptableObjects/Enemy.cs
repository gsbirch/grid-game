using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    [Header("Enemy Info")]
    public string enemyName;
    public int health;
    public Sprite initiativePicture;

    [Header("Enemy Abilities")]
    public List<EnemyAbility> abilities;
}
