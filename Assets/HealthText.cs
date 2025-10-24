using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    public EnemyTurnEntity enemyTurnEntity;

    private void Awake() {
        enemyTurnEntity.OnStatChanged += (stat, oldValue, newValue) => {
            if (stat == GameManager.HEALTH_STAT) {
                UpdateHealthText(newValue);
            }
        };
        UpdateHealthText(enemyTurnEntity.Enemy.health);
    }

    void UpdateHealthText(int newHealth) {
        var textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null) {
            textMesh.text = newHealth.ToString();
        }
    }
}
