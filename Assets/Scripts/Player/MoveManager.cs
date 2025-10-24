using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveManager : MonoBehaviour, ITileClick
{
    public string instructionsText = "Click a tile to move to it. Press ESC to cancel.";
    // The Move Manager handles the players movement ability

    // Start is called before the first frame update
    void Start()
    {
        HUDBehaviour.Instance.moveButton.clicked += () => {
            if (GameManager.Instance.GameState == GameState.PlayerTurnMovement) {
                // Cancel movement mode if already in it
                GameManager.Instance.GameState = GameState.PlayerTurnDefault;
                return;
            }

            GameManager.Instance.GameState = GameState.PlayerTurnMovement;

            HUDBehaviour.Instance.ShowOverlay(instructionsText, Color.yellow);
            Player.Instance.OnCellClicked += TileClicked;
            Player.Instance.OnCellHovered += TileLook;
        };

        GameManager.Instance.OnGameStateChanged += (previousState, newState) => {
            // The HUD manager now hides the overlay on any state change
            if (newState == GameState.PlayerTurnMovement || newState == GameState.PlayerTurnDefault) {
                HUDBehaviour.Instance.moveButton.style.display = DisplayStyle.Flex;
            }
            else {
                HUDBehaviour.Instance.moveButton.style.display = DisplayStyle.None;
            }

            Player.Instance.OnCellClicked -= TileClicked;
            Player.Instance.OnCellHovered -= TileLook;
        };
    }
    public void TileClicked(Vector3Int pos) {
        var currPos = GetComponent<TileEntity>().CellPosition;
        int distance = Utilities.Distance(currPos, pos);
        if (distance == 0) {
            HUDBehaviour.Instance.ShowNotification("You are already there!", Color.red);
            return;
        }
        if (GetComponent<PlayerTurnEntity>().GetStat(GameManager.MOVEMENT_STAT) < distance) {
            HUDBehaviour.Instance.ShowNotification("Not enough movement points to move there!", Color.red);
            return;
        }

        GetComponent<TileEntity>().UpdateCellPosition(pos);
        GetComponent<PlayerTurnEntity>().ModifyStat(GameManager.MOVEMENT_STAT, -distance);

        // This may become an option later
        GameManager.Instance.GameState = GameState.PlayerTurnDefault;
    }

    public void TileLook(Vector3Int pos) {
        if (pos == Utilities.NULLVEC) {
            HUDBehaviour.Instance.HideInfo();
            return;
        }

        var currPos = GetComponent<TileEntity>().CellPosition;
        int distance = Utilities.Distance(currPos, pos);

        HUDBehaviour.Instance.ShowInfo($"This move costs {distance}");
    }
}
