using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Utilities
{
    public static readonly Vector3Int NULLVEC = new Vector3Int(-99, -99, -99); 
    public static readonly Vector3Int[] directions = new Vector3Int[] {
        // pointy topped, odd-r offset
        new Vector3Int(+1, 0, 0), new Vector3Int(0, +1, 0), new Vector3Int(-1, +1, 0),
        new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(+1, -1, 0)
    };
    public static bool VerifyVectorNotNull(Vector3Int pos) {
        if (pos != NULLVEC) return true;
        Debug.LogError("Vector is NULLVEC");
        return false;
    }

    /// <summary>
    /// Hex distance for a pointy-top odd-r offset grid (parity based on row = y).
    /// Very small, allocation-free, and O(1).
    /// </summary>
    public static int Distance(Vector3Int a, Vector3Int b) {
        // odd-r (pointy-top) offset -> cube
        int ax = a.x - (a.y - (a.y & 1)) / 2;
        int az = a.y;
        int ay = -ax - az;

        int bx = b.x - (b.y - (b.y & 1)) / 2;
        int bz = b.y;
        int by = -bx - bz;

        int dx = Mathf.Abs(ax - bx);
        int dy = Mathf.Abs(ay - by);
        int dz = Mathf.Abs(az - bz);

        // cube distance = max(|dx|,|dy|,|dz|)
        return Math.Max(dx, Math.Max(dy, dz));
    }


    public static Sprite GetSprite(this TileBase tileBase) {
        if (tileBase == null)
            return null;

        TileData tileData = new TileData();
        // Populate tileData with this tile’s info
        tileBase.GetTileData(Vector3Int.zero, null, ref tileData);
        return tileData.sprite;
    }

    // Fisher-Yates shuffle algorithm
    public static List<T> Shuffle<T>(this IList<T> list) {
        List<T> shuffledList = new(list); // Create a new list to hold the shuffled elements
        int n = shuffledList.Count;
        for (int i = n - 1; i > 0; i--) {
            int j = UnityEngine.Random.Range(0, i + 1); // Use Unity's Random.Range to get a random index
            (shuffledList[j], shuffledList[i]) = (shuffledList[i], shuffledList[j]);
        }
        return shuffledList;
    }

    // Better invoke
    public static void Invoke(this MonoBehaviour mb, Action f, float delay) {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }

    private static IEnumerator InvokeRoutine(System.Action f, float delay) {
        yield return new WaitForSeconds(delay);
        f();
    }

    // Performs a DFS to go through entire transform tree for the tag
    public static IEnumerable<GameObject> FindChildrenWithTag(GameObject parent, string tag) {
        Stack<Transform> searchStack = new();

        searchStack.Push(parent.transform);
        while (searchStack.Count > 0) {
            var curr = searchStack.Pop();
            foreach (Transform t in curr) {
                searchStack.Push(t);
                if (t.CompareTag(tag)) yield return t.gameObject;
            }
        }
    }
}
