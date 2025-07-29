using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapTile : MonoBehaviour
{
    Color exhausted = new Color(1, 1, 1, 0.3f);
    Color unoccupied = new Color(1, 1, 0, 0.3f);
    Color occupied = new Color(0.7568628f, 0.2235294f, 0.9411765f, 0.3f);

    SpriteRenderer tileRenderer;

    public void InitMapTile(TileType shape)
    {
        tileRenderer = GetComponentInChildren<SpriteRenderer>();
        tileRenderer.sprite = Tile.TILE_SPRITES[shape];
        tileRenderer.color = unoccupied;
    }

    public void TileActive()
    {
        tileRenderer.color = occupied;
    }

    public void TileInactive() 
    { 
        tileRenderer.color = unoccupied;
    }

    public void ExhaustTile()
    {
        unoccupied = exhausted;
    }
}
