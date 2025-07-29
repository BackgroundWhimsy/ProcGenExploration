using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    public static TileManager GetTileManager()
    {
        if (Instance == null)
        {
            // try to get the Manager from the Level first
            Instance = (TileManager)FindAnyObjectByType(typeof(TileManager));

            // if it still doesn't exist, make a new one
            if(Instance == null)
            {
                Instance = new TileManager();
            }
            
        }

        return Instance;
    }

    [SerializeField] GameObject startingTile;
    GameObject currentTile;
    CameraMovement tileCamera;
    List<GameObject> tiles = new List<GameObject>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject tile in tiles) {
            tile.SetActive(false);
        }

        currentTile = startingTile;
        currentTile.SetActive(true);
    }

    public void RegisterTile(GameObject tile)
    {
        tiles.Add(tile);
    }
}
