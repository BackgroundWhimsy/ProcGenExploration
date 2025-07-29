using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // dimensions should be odd, minimum of 3
    public int ROWS = 5;
    public int COLS = 5;
    public int MIN_TILES = 15;

    private Tile[,] levelGrid;
    Tuple<int, int> startingTile;

    private TileBehavior[,] tileGrid;          // pointers between tile prefabs
    private GameObject levelTiles;          // parent prefab for all tile prefabs
    private GameObject mapTiles;            // parent prefab for all minimap prefabs

    private int numTiles = 0;

    private int[,] coinMap;

    public void InstantiateLevel()
    {
        levelTiles = GameObject.Find("LevelTiles");
        mapTiles = GameObject.Find("MapTiles");

        Destroy(levelTiles);
        Destroy(mapTiles);

        levelTiles = new GameObject();
        levelTiles.name = "LevelTiles";

        mapTiles = new GameObject();
        mapTiles.name = "MapTiles";

        tileGrid = new TileBehavior[ROWS, COLS];

        GenerateLevel();

        // turn the tiles in the grid into prefabs in the scene
        SpawnPrefab(startingTile.Item1, startingTile.Item2);
    }

    public void GenerateLevel()
    {
        numTiles = 0;
        while (numTiles < MIN_TILES)
        {
            levelGrid = new Tile[ROWS, COLS];
            numTiles = 0;

            // choose a random starting point somewhere in the middle of the grid
            int centerRow = (ROWS / 2);
            int row = UnityEngine.Random.Range(centerRow - 1, centerRow + 2);
            int centerCol = (COLS / 2);
            int col = UnityEngine.Random.Range(centerCol - 1, centerCol + 2);

            // build the level
            BuildMaze(row, col);
        
            // save the starting tile
            startingTile = new Tuple<int, int>(row, col);
        }

        coinMap = GetComponent<CoinMapGenerator>().GenerateCoinMap();

        // count up the coins in the level
        int coins = 0;
        for (int i = 0; i < ROWS; i++) {
            for (int j = 0; j < COLS; j++) {
                if (levelGrid[i,j] != null)
                {
                    coins += coinMap[i, j];
                }
            }
        }
        

        // print the grid for debugging
        //Debug.Log(startingTile.ToString());
        PrintGrid();
        Debug.Log(coins);
    }

    void BuildMaze(int row, int col)
    {
        // fill in the tile at the current coordinate
        Tile currentTile = GetValidTile(row, col);
        levelGrid[row, col] = currentTile;
        numTiles++;

        // fill in the tiles around the current coordinate
        if(currentTile.up && row - 1 >= 0 && levelGrid[row - 1, col] == null)
        {
            BuildMaze(row - 1, col);
        }
        if (currentTile.down && row + 1 < ROWS && levelGrid[row + 1, col] == null)
        {
            BuildMaze(row + 1, col);
        }
        if (currentTile.left && col - 1 >= 0 && levelGrid[row, col - 1] == null)
        {
            BuildMaze(row, col - 1);
        }
        if (currentTile.right && col + 1 < COLS && levelGrid[row, col + 1] == null)
        {
            BuildMaze(row, col + 1);
        }
    }

    void SpawnPrefab(int row, int col)
    {
        // get the tile
        Tile tile = levelGrid[row, col];

        // get the prefab
        GameObject tilePrefab = Tile.TILE_PREFABS[tile.type];

        // get the world space coordinates
        float xPos = (col - startingTile.Item2) * Tile.WIDTH;
        float zPos = (startingTile.Item1 - row) * Tile.WIDTH;

        // create the tile
        GameObject tileInstance = Instantiate(tilePrefab, levelTiles.transform);
        tileInstance.transform.parent = levelTiles.transform;
        tileInstance.transform.position = new Vector3(xPos, levelTiles.transform.position.y, zPos);

        // store it in the grid
        TileBehavior instanceLinks = tileInstance.GetComponent<TileBehavior>();
        tileGrid[row, col] = instanceLinks;

        if(row == startingTile.Item1 && col == startingTile.Item2)
        {
            instanceLinks.isStartingTile = true;

        }

        // create the map icon and connect it to the tile 
        instanceLinks.mapTile = SpawnMapIcon(tileInstance.transform.position, tile.type);

        // spawn the entities on the tile
        instanceLinks.SpawnEntities(coinMap[row, col]);

        // create the tiles for all the open doorways
        if (tile.up)
        {
            // check if a tile already exists in that direction
            if (tileGrid[row - 1, col] != null)
            {
                // connect them
                TileBehavior upTile = tileGrid[row - 1, col];
                upTile.down = tileInstance;
                instanceLinks.up = upTile.gameObject;
            }
            else
            {
                // make a new tile
                SpawnPrefab(row - 1, col);
            }
        }
        if (tile.down)
        {
            // check if a tile already exists in that direction
            if (tileGrid[row + 1, col] != null)
            {
                // connect them
                TileBehavior downTile = tileGrid[row + 1, col];
                downTile.up = tileInstance;
                instanceLinks.down = downTile.gameObject;
            }
            else
            {
                // make a new tile
                SpawnPrefab(row + 1, col);
            }
        }
        if (tile.left)
        {
            // check if a tile already exists in that direction
            if (tileGrid[row, col - 1] != null)
            {
                // connect them
                TileBehavior leftTile = tileGrid[row, col - 1];
                leftTile.right = tileInstance;
                instanceLinks.left = leftTile.gameObject;
            }
            else
            {
                // make a new tile
                SpawnPrefab(row, col - 1);
            }
        }
        if (tile.right)
        {
            // check if a tile already exists in that direction
            if (tileGrid[row, col + 1] != null)
            {
                // connect them
                TileBehavior rightTile = tileGrid[row, col + 1];
                rightTile.left = tileInstance;
                instanceLinks.right = rightTile.gameObject;
            }
            else
            {
                // make a new tile
                SpawnPrefab(row, col + 1);
            }
        }
    }

    MiniMapTile SpawnMapIcon(Vector3 pos, TileType type)
    {
        GameObject mapTile = Instantiate(Resources.Load<GameObject>("Tiles/MiniMapTile"));
        mapTile.transform.parent = mapTiles.transform;
        mapTile.transform.position = pos;
        mapTile.SetActive(false);   // all map tiles are disabled until the first time the player enters them

        MiniMapTile mapScript = mapTile.GetComponent<MiniMapTile>();
        mapScript.InitMapTile(type);

        return mapScript;
    }

    Tile GetValidTile(int row, int col)
    {
        // make a list with all the tiles in it
        List<Tile> validTiles = new List<Tile>();
        foreach(Tile tile in Tile.ALL)
        {
            validTiles.Add(tile);
        }

        // check the tiles in all four cardinal directions for requirements
        List<Direction> mustHave = new List<Direction>();
        List<Direction> mustAvoid = new List<Direction>();

        int upRow = row - 1;
        int downRow = row + 1;
        int leftCol = col - 1;
        int rightCol = col + 1;

        // up
        if (upRow < 0) // out of bounds
        {
            mustAvoid.Add(Direction.Up);
        }
        else
        {
            // tile exists
            if (levelGrid[upRow, col] != null)
            {
                // tile has a doorway leading back to current coordinate
                if(levelGrid[upRow, col].down)
                {
                    mustHave.Add(Direction.Up);
                }
                else
                {
                    mustAvoid.Add(Direction.Up);
                }
            }
        }

        // down
        if (downRow >= ROWS) // out of bounds
        {
            mustAvoid.Add(Direction.Down);
        }
        else
        {
            // tile exists
            if (levelGrid[downRow, col] != null)
            {
                // tile has a doorway leading back to current coordinate
                if (levelGrid[downRow, col].up)
                {
                    mustHave.Add(Direction.Down);
                }
                else
                {
                    mustAvoid.Add(Direction.Down);
                }
            }
        }

        // left
        if (leftCol < 0) // out of bounds
        {
            mustAvoid.Add(Direction.Left);
        }
        else
        {
            // tile exists
            if (levelGrid[row, leftCol] != null)
            {
                // tile has a doorway leading back to current coordinate
                if (levelGrid[row, leftCol].right)
                {
                    mustHave.Add(Direction.Left);
                }
                else
                {
                    mustAvoid.Add(Direction.Left);
                }
            }
        }

        // right
        if (rightCol >= COLS) // out of bounds
        {
            mustAvoid.Add(Direction.Right);
        }
        else
        {
            // tile exists
            if (levelGrid[row, rightCol] != null)
            {
                // tile has a doorway leading back to current coordinate
                if (levelGrid[row, rightCol].left)
                {
                    mustHave.Add(Direction.Right);
                }
                else
                {
                    mustAvoid.Add(Direction.Right);
                }
            }
        }

        List<Tile> upTiles = new List<Tile>(Tile.UP_TILES);
        List<Tile> downTiles = new List<Tile>(Tile.DOWN_TILES);
        List<Tile> leftTiles = new List<Tile>(Tile.LEFT_TILES);
        List<Tile> rightTiles = new List<Tile>(Tile.RIGHT_TILES);

        // Intersect the list of valid tiles with each list of tiles that match the Must Have Directions
        foreach (Direction dir in mustHave) 
        {
            switch (dir)
            {
                case Direction.Up:   
                    IntersectTiles(validTiles, upTiles);
                    break;
                case Direction.Down:
                    IntersectTiles(validTiles, downTiles);
                    break;
                case Direction.Left:
                    IntersectTiles(validTiles, leftTiles);
                    break;
                case Direction.Right:
                    IntersectTiles(validTiles, rightTiles);
                    break;
            }
        }

        // Subtract each list of Must Avoid tiles from the list of valid tiles
        foreach (Direction dir in mustAvoid)
        {
            switch (dir)
            {
                case Direction.Up:
                    SubtractTiles(validTiles, upTiles);
                    break;
                case Direction.Down:
                    SubtractTiles(validTiles, downTiles);
                    break;
                case Direction.Left:
                    SubtractTiles(validTiles, leftTiles);
                    break;
                case Direction.Right:
                    SubtractTiles(validTiles, rightTiles);
                    break;
            }
        }

        // select a random tile from the final list of valid tiles
        int tileIndex = UnityEngine.Random.Range(0, validTiles.Count);

        return validTiles[tileIndex];
    }

    void SubtractTiles(List<Tile> validTiles, List<Tile> directionalTiles)
    {
        foreach (Tile tile in directionalTiles)
        {
            validTiles.Remove(tile);
        }
    }

    void IntersectTiles(List<Tile> validTiles, List<Tile> directionalTiles)
    {
        List<Tile> toRemove = new List<Tile>();
        foreach (Tile tile in validTiles)
        {
            if (!directionalTiles.Contains(tile))
            {
                toRemove.Add(tile);
            }
        }

        foreach(Tile tile in toRemove)
        {
            validTiles.Remove(tile);
        }
    }

    public Tuple<int, int> GetStartingCoordinates()
    {
        return startingTile;
    }

    public Tile[,] GetLevelGrid()
    {
        return levelGrid; 
    }

    public TileBehavior[,] GetTileGrid()
    {
        return tileGrid;
    } 

    /*public void PrintTileList(List<Tile> list)
    {
        string tiles = "";
        foreach (Tile tile in list) { 
            tiles += tile.ToString() + " ";
        }
        Debug.Log(tiles);
    }

    public void PrintDirectionList(List<Direction> list)
    {
        string directions = "";
        foreach (Direction dir in list)
        {
            directions += dir.ToString() + " ";
        }
        Debug.Log(directions);
    }*/

    public void PrintGrid()
    {
        // create a string
        string grid = "Num Tiles: " + numTiles;
        grid += "\n---------------------\n";
        for(int i = 0; i < levelGrid.GetLength(0); i++) 
        {
            grid += "| ";
            for(int j = 0; j < levelGrid.GetLength(1); j++)
            {
                if (levelGrid[i,j] != null)
                {
                    grid += levelGrid[i,j].ToString();
                }
                else
                {
                    grid += "00";
                }
                grid += " | ";
            }
            grid += "\n---------------------\n";
        }

        // print it out
        Debug.Log(grid);
    }
}
