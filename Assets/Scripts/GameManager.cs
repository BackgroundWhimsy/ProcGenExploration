using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager Instance;
    public static GameManager GetGameManager()
    {
        if (Instance == null)
        {
            Instance = new GameManager();
        }
        return Instance;
    }

    public LevelGenerator levelGen;

    [SerializeField] CameraMovement tileCamera;
    [SerializeField] GameObject mapCamera;

    private PlayerMovement player;
    private GameObject currentTile;

    Vector3 MAP_CAMERA_START = new Vector3(8, 55, 8);
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Start()
    {
        levelGen = GetComponent<LevelGenerator>();
        player = PlayerMovement.GetPlayer();

        ResetGame();
    }

    private void ResetGame()
    {
        // set up the level
        levelGen.InstantiateLevel();

        TileBehavior[,] tileGrid = levelGen.GetTileGrid();
        Tuple<int, int> startingTileCoords = levelGen.GetStartingCoordinates();

        // deactivate every tile except the starting tile
        foreach (TileBehavior tile in tileGrid) {
            if (tile != null) { 
                tile.gameObject.SetActive(false);
            }
        }

        currentTile = tileGrid[startingTileCoords.Item1, startingTileCoords.Item2].gameObject;
        currentTile.SetActive(true);

        // place the player at the start
        float xPos = currentTile.transform.position.x + (Tile.WIDTH / 2);
        float zPos = currentTile.transform.position.z + (Tile.WIDTH / 2);
        player.gameObject.transform.position = new Vector3(xPos, 2, zPos);

        // move the camera to the start
        tileCamera.MoveCamera(currentTile.transform.position);

        // move the map camera to the center of the grid
        int xDiff = startingTileCoords.Item2 - (levelGen.COLS / 2);
        int zDiff = startingTileCoords.Item1 - (levelGen.ROWS / 2);

        xPos = MAP_CAMERA_START.x + (Tile.WIDTH * -xDiff);
        zPos = MAP_CAMERA_START.z + (Tile.WIDTH * zDiff);
        mapCamera.transform.position = new Vector3( xPos, mapCamera.transform.position.y, zPos);
        
    }
    
    // Activate the tile the player is moving to and deactivate the old tile
    public void ShiftTile(Direction direction)
    {
        GameObject newTile = currentTile.GetComponent<TileBehavior>().GetLink(direction);

        if (newTile != null)
        {
            currentTile.SetActive(false);
            newTile.SetActive(true);

            currentTile = newTile;
            tileCamera.MoveCamera(currentTile.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetGame();
        }
    }
}
