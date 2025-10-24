using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The Initiative Manager is responsible for handling turns
// and the order of actions in combat.
public class InitiativeManager : MonoBehaviour
{
    // singleton
    public static InitiativeManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            initiativeOrder = new List<ITurnEntity>();
        } else {
            Destroy(gameObject);
        }
    }

    public List<ITurnEntity> initiativeOrder;
    public int initiativeIndex = 0;

    public void PassTurn() {
        var currentEntity = initiativeOrder[initiativeIndex];
        currentEntity.EndOfTurn();

        initiativeIndex = (initiativeIndex + 1) % initiativeOrder.Count;

        currentEntity = initiativeOrder[initiativeIndex];
        currentEntity.StartOfTurn();
        currentEntity.TakeTurn();

        HUDBehaviour.Instance.UpdateUI();
    }

    public void RegisterEntity(ITurnEntity entity) {
        initiativeOrder.Add(entity);
        HUDBehaviour.Instance.UpdateUI();
    }

    public void UnregisterEntity(ITurnEntity entity) {
        initiativeOrder.Remove(entity);
        HUDBehaviour.Instance.UpdateUI();
    }
}
