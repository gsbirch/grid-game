using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class TileEntity : MonoBehaviour
{
    // A tile entity is an entity that exists on the tilemap
    // They can be moved and know their location on the grid

    public Vector3Int startPosition = Utilities.NULLVEC;

    //[HideInInspector]
    public Vector3Int CellPosition = Utilities.NULLVEC;

    public void UpdateCellPosition(Vector3Int newCellPosition) {
        if (newCellPosition == Utilities.NULLVEC) {
            Debug.LogError("Cannot move TileEntity to NULLVEC");
            return;
        }
        if (CellPosition == newCellPosition) {
            return; // no need to update
        }
        if (TilemapManager.Instance.tileEntities.ContainsKey(newCellPosition) && TilemapManager.Instance.tileEntities[newCellPosition] != null) {
            Debug.LogError("Cannot move TileEntity to occupied position: " + newCellPosition);
            return;
        }

        var oldCellPos = CellPosition;
        CellPosition = newCellPosition;

        TilemapManager.Instance.tileEntities[oldCellPos] = null; // remove old entry
        if (!TilemapManager.Instance.tileEntities.ContainsKey(newCellPosition)) {
            TilemapManager.Instance.tileEntities.Add(newCellPosition, this);
        } else {
            TilemapManager.Instance.tileEntities[newCellPosition] = this;
        }

        var newPos = TilemapManager.Instance.CellToWorld(newCellPosition);
        newPos.z = -1f;
        transform.position = newPos;
    }

    private void Start() {
        UpdateCellPosition(startPosition);
    }

    public void SetHighlight(bool highlight) {
        /*var sr = GetComponent<SpriteRenderer>();
        if (sr == null) {
            return;
        }
        if (highlight) {
            sr.color = Color.yellow;
        } else {
            sr.color = Color.white;
        }*/
    }

    private void OnDestroy() {
        if (TilemapManager.Instance != null && TilemapManager.Instance.tileEntities.ContainsKey(CellPosition) && TilemapManager.Instance.tileEntities[CellPosition] == this) {
            TilemapManager.Instance.tileEntities[CellPosition] = null;
        }
    }
}
