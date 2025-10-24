using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetManager : MonoBehaviour, ITileClick
{
    // Singleton
    public static TargetManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    // The Move Manager handles the players movement ability
    Card currentCard = null;
    int range = 0;
    int amount = 0;

    List<ITargettable> validTargets = new();
    List<Vector3Int> validTargetPositions = new();

    List<ITargettable> targets = new();

    // Start is called before the first frame update
    void Start() {

        GameManager.Instance.OnGameStateChanged += (previousState, newState) => {
            Player.Instance.OnCellClicked -= TileClicked;
            //Player.Instance.OnCellHovered -= TileLook;
            HUDBehaviour.Instance.confirmButton.clicked -= ConfirmTargets;
        };
    }

    public void InitalizeTargetSelection(Vector3Int position, Card c, int bonusTargets) {
        currentCard = c;
        string a = c.targets;
        int r = c.range;

        if (a == "SELF") {
            GameManager.Instance.GameState = GameState.PlayerTurnTarget;
            HUDBehaviour.Instance.ShowCard(c);
            HUDBehaviour.Instance.confirmButton.style.display = DisplayStyle.Flex;
            HUDBehaviour.Instance.confirmButton.clicked += ConfirmTargets;

            string instructionsText = $"{c.cardName} targets yourself, click the button to confirm. Press ESC to cancel.";


            HUDBehaviour.Instance.ShowOverlay(instructionsText, Color.red);
        }
        else if (a == "ALL") {
            // Select all valid targets in range
        }
        else {
            amount = int.Parse(a) + bonusTargets;
            range = r;

            var entities = TilemapManager.Instance.GetTileEntitiesInRange(position, r);
            validTargets.Clear();
            validTargetPositions.Clear();
            foreach (var e in entities) {
                // check for a targettable component
                var targettable = e.GetComponent<ITargettable>();
                if (targettable != null) {
                    validTargets.Add(targettable);
                    validTargetPositions.Add(e.CellPosition);
                }
            }

            if (validTargets.Count == 0) {
                HUDBehaviour.Instance.ShowNotification("No valid targets in range!", Color.red);
                GameManager.Instance.GameState = GameState.PlayerTurnDefault;
                return;
            }

            GameManager.Instance.GameState = GameState.PlayerTurnTarget;
            HUDBehaviour.Instance.ShowCard(c);
            HUDBehaviour.Instance.confirmButton.style.display = DisplayStyle.Flex;
            HUDBehaviour.Instance.confirmButton.clicked += ConfirmTargets;

            foreach (var t in validTargets) {
                t.SetSelectionState(SelectArrowState.Valid);
            }

            string instructionsText = $"Select {(range == 1 ? "" : "up to")} {amount} {(amount == 1 ? "enemy" : "enemies")} within {range} tile{(range == 1 ? "s" : "")}. Press ESC to cancel.";


            HUDBehaviour.Instance.ShowOverlay(instructionsText, Color.red);
            Player.Instance.OnCellClicked += TileClicked;
            //Player.Instance.OnCellHovered += TileLook;
        }
    }

    public void TileClicked(Vector3Int pos) {
        // We already handled validation for all targets
        if (!validTargetPositions.Contains(pos)) {
            return;
        }
        var tileEntity = TilemapManager.Instance.tileEntities[pos];
        var target = tileEntity.GetComponent<EnemyTurnEntity>();

        if (targets.Contains(target)) {
            targets.Remove(target);
            target.SetSelectionState(SelectArrowState.Valid);
        } else {
            if (targets.Count >= amount) {
                HUDBehaviour.Instance.ShowNotification("Already selected maximum number of targets!", Color.red);
                return;
            }
            targets.Add(target);
            target.SetSelectionState(SelectArrowState.Selected);
        }
    }

    public void TileLook(Vector3Int pos) {
        // do nothing?
    }

    public void ConfirmTargets() {
        if (targets.Count == 0 && currentCard.targets != "SELF") {
            HUDBehaviour.Instance.ShowNotification("No targets selected!", Color.red);
            return;
        }

        PlayerTurnEntity.Instance.PlayCard(currentCard, targets);

        targets.Clear();
    }
}
