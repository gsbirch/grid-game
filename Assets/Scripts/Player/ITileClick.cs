using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileClick
{
    public void TileLook(Vector3Int pos);
    public void TileClicked(Vector3Int pos);
}
