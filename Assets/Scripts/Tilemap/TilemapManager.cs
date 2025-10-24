using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum TileLayer {
    Ground,
    Select
}

public class TilemapManager : MonoBehaviour
{
    public readonly Vector3Int[] evenOffsets = new Vector3Int[] {
        new( 1,  0, 0),
        new( 0, -1, 0),
        new( -1, -1, 0),
        new(-1,  0, 0),
        new(-1,  1, 0),
        new( 0,  1, 0)
    };

    public readonly Vector3Int[] oddOffsets = new Vector3Int[] {
         new( 1,  0, 0),
         new( 1, -1, 0),
         new( 0, -1, 0),
         new(-1,  0, 0),
         new( 0,  1, 0),
         new( 1,  1, 0),
    };

    [SerializeField]
    Tilemap groundLayer;
    [SerializeField]
    Tilemap selectLayer;

    public event Action<TileLayer, Vector3Int> OnTileChanged;

    public Dictionary<Vector3Int, TileEntity> tileEntities = new();

    public static TilemapManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        tileEntities.Add(Utilities.NULLVEC, null); // just to avoid key not found errors
    }

    public TileBase GetTile(TileLayer layer, Vector3Int pos) {
        switch (layer) {
            case TileLayer.Ground:
                return groundLayer.GetTile(pos);
            case TileLayer.Select:
                return selectLayer.GetTile(pos);
            default:
                Debug.LogError("Invalid tile layer specified: " + layer);
                return null;
        }
    }

    public void SetTile(TileLayer layer, Vector3Int pos, TileBase tile) {
        switch (layer) {
            case TileLayer.Ground:
                groundLayer.SetTile(pos, tile);
                OnTileChanged?.Invoke(layer, pos);
                break;
            case TileLayer.Select:
                selectLayer.SetTile(pos, tile);
                // there is no need to invoke OnTileChanged for select layer
                break;
            default:
                Debug.LogError("Invalid tile layer specified: " + layer);
                break;
        }
    }

    public Vector3Int[] GetOffsets(Vector3Int pos) {
        return (pos.y & 1) == 0 ? evenOffsets : oddOffsets;
    }

    public List<TileEntity> GetAdjacentTileEntities(Vector3Int pos) {
        List<TileEntity> adjacentEntities = new();
        // Pick correct offsets depending on row parity
        Vector3Int[] offsets = GetOffsets(pos);

        // Collect neighbors
        foreach (var offset in offsets) {
            Vector3Int neighborPos = pos + offset;
            if (TileIsOccupied(pos)) {
                adjacentEntities.Add(tileEntities[neighborPos]);
            }
        }

        return adjacentEntities;
    }

    public List<TileEntity> GetTileEntitiesInRange(Vector3Int startPos, int range) {
        List<TileEntity> entitiesInRange = new();

        // Convert start to cube
        int startCx = startPos.x - (startPos.y - (startPos.y & 1)) / 2;
        int startCz = startPos.y;
        int startCy = -startCx - startCz;

        for (int dx = -range; dx <= range; dx++) {
            for (int dy = Math.Max(-range, -dx - range); dy <= Math.Min(range, -dx + range); dy++) {
                int dz = -dx - dy;

                int cx = startCx + dx;
                int cy = startCy + dy;
                int cz = startCz + dz;

                // Convert back to offset (pointy-top odd-r)
                int ox = cx + (cz - (cz & 1)) / 2;
                int oy = cz;
                Vector3Int pos = new Vector3Int(ox, oy, 0);

                if (pos != startPos) { // skip center tile if desired
                    if (TileIsOccupied(pos)) {
                        entitiesInRange.Add(tileEntities[pos]);
                    }    
                }
            }
        }

        return entitiesInRange;
    }



    // Most classes shouldn't use this
    // But the player needs it to get the tile based on their mouse
    public Tilemap GetLayer(TileLayer layer) {
        switch (layer) {
            case TileLayer.Ground:
                return groundLayer;
            case TileLayer.Select:
                return selectLayer;
            default:
                Debug.LogError("Invalid tile layer specified: " + layer);
                return null;
        }
    }

    public Vector3Int WorldToCell(Vector3 worldPos) {
        return groundLayer.WorldToCell(worldPos);
    }
    public Vector3 CellToWorld(Vector3Int cellPos) {
        return groundLayer.CellToWorld(cellPos);
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal) {
        var openSet = new PriorityQueue<Vector3Int>();
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        var gScore = new Dictionary<Vector3Int, int> { [start] = 0 };
        var fScore = new Dictionary<Vector3Int, int> { [start] = Utilities.Distance(start, goal) };

        openSet.Enqueue(start, fScore[start]);

        while (openSet.Count > 0) {
            Vector3Int current = openSet.Dequeue();

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            foreach (var neighbor in GetNeighbors(current)) {
               
                if ((TileIsOccupied(neighbor) || groundLayer.GetTile(neighbor) == null) && neighbor != goal) // blocked by entity or empty ground tile
                    continue;

                int tentativeGScore = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor]) {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Utilities.Distance(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }
        return null; // no path found
    }

    private List<Vector3Int> GetNeighbors(Vector3Int pos) {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        var offsets = (pos.y & 1) == 0 ? evenOffsets : oddOffsets;

        foreach (var offset in offsets)
            neighbors.Add(pos + offset);

        return neighbors;
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current) {
        var path = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();

        /*if (path.Count > 0)
            path.RemoveAt(0); // remove start node, keep only steps after*/

        return path;
    }

    public bool TileIsOccupied(Vector3Int pos) {
        return tileEntities.ContainsKey(pos) && tileEntities[pos] != null;
    }

}

/// <summary>
/// Simple priority queue for A*
/// </summary>
public class PriorityQueue<T> {
    private List<(T item, int priority)> elements = new List<(T, int)>();

    public int Count => elements.Count;

    public void Enqueue(T item, int priority) {
        elements.Add((item, priority));
    }

    public T Dequeue() {
        int bestIndex = 0;
        for (int i = 1; i < elements.Count; i++)
            if (elements[i].priority < elements[bestIndex].priority)
                bestIndex = i;

        T bestItem = elements[bestIndex].item;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }

    public bool Contains(T item) {
        foreach (var e in elements)
            if (EqualityComparer<T>.Default.Equals(e.item, item))
                return true;
        return false;
    }
}