/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RemoveButton : MonoBehaviour, ITileClick {
    
    public string instructionsText = "NULL_TEXT RemoveButton";

    private void Start() {
        // Set the button text to the zoning type
        HUDBehaviour.Instance.removeButton.clicked += () => {
            // This is a cheeky way of ensuring everyone unsubscribes from the event before subscribing again
            // There might be a better way to do this
            GameManager.Instance.GameState = GameState.Default;

            GameManager.Instance.GameState = GameState.Zoning;
            HUDBehaviour.Instance.ShowOverlay(instructionsText, Color.red);
            Player.Instance.OnCellClicked += TileClicked;
        };

        GameManager.Instance.OnGameStateChanged += (previousState, newState) => {
            if (newState != GameState.Zoning) {
                Player.Instance.OnCellClicked -= TileClicked;
            }
        };
    }

    public void TileClicked(Vector3Int pos) {
        if (TilemapManager.Instance.GetTile(TileLayer.Building, pos) != null) {
            TilemapManager.Instance.SetTile(TileLayer.Building, pos, null);
            HUDBehaviour.Instance.ShowNotification("Building removed!", Color.green);
        } 
        else if (TilemapManager.Instance.GetTile(TileLayer.Ground, pos) != null) {
            if (TilemapManager.Instance.GetTile(TileLayer.Ground, pos) != TilemapManager.Instance.roadTile) return;
            foreach (Vector3Int adjPos in TilemapManager.Instance.GetAdjacentTilePositions(pos)) {
                TileBase adjTile = TilemapManager.Instance.GetTile(TileLayer.Building, adjPos);
                if (adjTile == null) continue;
                if (GetRoadCount(adjPos) == 1) {
                    HUDBehaviour.Instance.ShowNotification("A building depends on this road!", Color.red);
                    return;
                }
            }
            TilemapManager.Instance.SetTile(TileLayer.Ground, pos, TilemapManager.Instance.groundTile);
            HUDBehaviour.Instance.ShowNotification("Road tile removed!", Color.green);
        } else {
            HUDBehaviour.Instance.ShowNotification("It'd be umm, kind of hard to remove the void", Color.cyan);
        }
    }

    public int GetRoadCount(Vector3Int pos) {
        List<TileBase> groundTiles = TilemapManager.Instance.GetAdjacentTiles(pos, TileLayer.Ground);
        int roadCount = 0;
        for (int i = 0; i < groundTiles.Count; i++) {
            if (groundTiles[i] == TilemapManager.Instance.roadTile) {
                roadCount++;
            }
        }
        return roadCount;
    }

    public void TileLook(Vector3Int pos) {
        return;
    }
}
*/