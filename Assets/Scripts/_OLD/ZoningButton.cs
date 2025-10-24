using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZoningButton : MonoBehaviour, ITileClick
{
    public string zoningType;
    public float cost;
    public string instructionsText;
    public TileBase tile;
    public bool onGroundLayer;

    private void Start() {
        /*// Set the button text to the zoning type
        HUDBehaviour.Instance.zoneButtons[zoningType].clicked += () => {
            // This is a cheeky way of ensuring everyone unsubscribes from the event before subscribing again
            // There might be a better way to do this
            GameManager.Instance.GameState = GameState.Default;

            GameManager.Instance.GameState = GameState.Zoning;
            HUDBehaviour.Instance.ShowOverlay(instructionsText, Color.yellow);
            Player.Instance.OnCellClicked += TileClicked;
            Player.Instance.OnCellHovered += TileLook;
        };

        GameManager.Instance.OnGameStateChanged += (previousState, newState) => {
            if (newState != GameState.Zoning) {
                Player.Instance.OnCellClicked -= TileClicked;
                Player.Instance.OnCellHovered -= TileLook;
            }
        };*/
    }

    public void TileClicked(Vector3Int pos) {
        /*if (CityManager.Instance.Money < cost) {
            HUDBehaviour.Instance.ShowNotification("Not enough money to zone " + zoningType + "!", Color.red);
            return;
        }

        if (!TilemapManager.Instance.TileAdjacentToRoad(pos)) {
            HUDBehaviour.Instance.ShowNotification(zoningType + " must be placed adjacent to a road!", Color.red);
            return;
        }

        TileBase groundTile = TilemapManager.Instance.GetTile(TileLayer.Ground, pos);
        
        var topTile = GetTopTile(pos);

        if (onGroundLayer) {
            if (groundTile == TilemapManager.Instance.groundTile && topTile == null) {
               
                TilemapManager.Instance.SetTile(TileLayer.Ground, pos, tile);
                CityManager.Instance.Money -= cost;
            } else {
                HUDBehaviour.Instance.ShowNotification("Cannot zone " + zoningType + " here!", Color.red);
            }
        } else {
            if (groundTile == TilemapManager.Instance.groundTile && topTile == null) {
                // In this scenario, we are zoning because there is nothing on the building layer
                TilemapManager.Instance.SetTile(TileLayer.Zones, pos, tile);
                CityManager.Instance.Money -= cost;
            } else if (topTile != null) {
                string tileName = topTile.name;
                if (tileName.StartsWith(Utilities.buildingPrefix)) {
                    HUDBehaviour.Instance.ShowNotification("There is already a building here!", Color.red);
                }
                if (tileName == zoningType) {
                    // This will change with the addition of infrastructure
                    HUDBehaviour.Instance.ShowNotification("You already zoned this area for " + zoningType + "!", Color.red);
                } else {
                    Building b = GetBuilding(tileName);
                    TilemapManager.Instance.SetTile(TileLayer.Zones, pos, null);
                    TilemapManager.Instance.SetTile(TileLayer.Building, pos, b.tileBase);
                    CityManager.Instance.Money -= cost;

                }
            } else {
                HUDBehaviour.Instance.ShowNotification("Cannot zone " + zoningType + " here!", Color.red);
            }
        }*/
    }

    public void TileLook(Vector3Int pos) {
        /*var tile = GetTopTile(pos);
        if (tile != null) {
            Building b = GetBuilding(tile.name);
            if (b == null) {
                HUDBehaviour.Instance.HideBuildingInfo();
            }
            else HUDBehaviour.Instance.ShowBuildingInfo(b);
        }
        else {
            HUDBehaviour.Instance.HideBuildingInfo();
        }*/
    }

    TileBase GetTopTile(Vector3Int pos) {
        return null;
        /*TileBase topTile = TilemapManager.Instance.GetTile(TileLayer.Zones, pos);
        if (topTile == null) {
            topTile = TilemapManager.Instance.GetTile(TileLayer.Building, pos);
        }
        return topTile;*/
    }
}
