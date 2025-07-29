using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehavior : MonoBehaviour
{
    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;

    public MiniMapTile mapTile;

    public bool isStartingTile = false;

    private int numCoins;
    private CoinPattern coinPattern = CoinPattern.None;

    public void SpawnEntities(int numCoins)
    {
        this.numCoins = numCoins;
        SpawnCoins();

        SpawnFurniture();
    }

    void SpawnCoins()
    {
        if (numCoins > 0)
        {
            // get list of possible coin prefabs
            CoinPattern[] coinPatterns = TileEntities.coinPrefabs[numCoins];

            // pick random coin prefab from the list and instantiate it
            int coinIndex = Random.Range(0, coinPatterns.Length);
            coinPattern = coinPatterns[coinIndex];
            GameObject coinPrefab = Instantiate(TileEntities.COIN_PREFABS[coinPattern]);
            coinPrefab.transform.parent = gameObject.transform;
            coinPrefab.transform.localPosition = Vector3.zero;

            CoinBehavior[] coins = coinPrefab.GetComponentsInChildren<CoinBehavior>();
            foreach (CoinBehavior coin in coins)
            {
                coin.SetOwner(this);
            }
        }
        else
        {
            mapTile.ExhaustTile();
        }
    }

    void SpawnFurniture()
    {
        // get list of forbidden locations
        HashSet<FloorLocation> takenSpots = new HashSet<FloorLocation>(TileEntities.COIN_LOCATIONS[coinPattern]);

        // make sure the player won't be trapped inside the furniture when the level starts
        if (isStartingTile && !takenSpots.Contains(FloorLocation.Center))
        {
            takenSpots.Add(FloorLocation.Center);
        }

        // get list of all furniture
        HashSet<GameObject> possibleFurniture = new HashSet<GameObject>(TileEntities.allFurniture);

        // remove all furniture that would clash with the coins
        foreach(FloorLocation location in takenSpots)
        {
            possibleFurniture.ExceptWith(TileEntities.FURNITURE_LOCATIONS[location]);
        }

        // choose randomly from the remaining layouts
        int index = Random.Range(0, possibleFurniture.Count);
        GameObject[] layouts = new GameObject[possibleFurniture.Count];
        possibleFurniture.CopyTo(layouts);

        GameObject chosenLayout = Instantiate(layouts[index]);
        chosenLayout.transform.parent = gameObject.transform;
        chosenLayout.transform.localPosition = Vector3.zero;
    }

    void OnDisable()
    {
        if (mapTile != null)
        {
            mapTile.TileInactive();
        }
    }

    void OnEnable()
    {
        if (mapTile != null)
        {
            if (!mapTile.gameObject.activeSelf)
            {
                mapTile.gameObject.SetActive(true);
            }
            
            mapTile.TileActive();
        }    
    }

    public void RemoveCoin()
    {
        numCoins--;
        if(numCoins <= 0)
        {
            mapTile.ExhaustTile();
        }
    }
    
    // get the GameObject in the direction indicated by the enum
    public GameObject GetLink(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return up;
            case Direction.Down:
                return down;
            case Direction.Left:
                return left;
            case Direction.Right:
                return right;
            default:
                return null;
        }
    }

    public static Direction StringToDirection(string directionTag)
    {
        switch (directionTag)
        {
            case "Up":
                return Direction.Up;
            case "Down":
                return Direction.Down;
            case "Left":
                return Direction.Left;
            case "Right":
                return Direction.Right;
            default:
                return Direction.Null;

        }
    }
}

public enum CoinPattern
{
    None, A1, A3, B3, C3, D3, A4, B4, A5, B5
}

public enum FloorLocation
{
    MidLeft, Center, MidRight, TopLeft, MidTop, TopRight, BottomLeft, MidBottom, BottomRight
}

public static class TileEntities
{
    public static Dictionary<CoinPattern, GameObject> COIN_PREFABS = new Dictionary<CoinPattern, GameObject>()
    {
        {CoinPattern.A1, Resources.Load<GameObject>("TileEntities/Coins/1A")},
        {CoinPattern.A3, Resources.Load<GameObject>("TileEntities/Coins/3A")},
        {CoinPattern.B3, Resources.Load<GameObject>("TileEntities/Coins/3B")},
        {CoinPattern.C3, Resources.Load<GameObject>("TileEntities/Coins/3C")},
        {CoinPattern.D3, Resources.Load<GameObject>("TileEntities/Coins/3D")},
        {CoinPattern.A4, Resources.Load<GameObject>("TileEntities/Coins/4A")},
        {CoinPattern.B4, Resources.Load<GameObject>("TileEntities/Coins/4B")},
        {CoinPattern.A5, Resources.Load<GameObject>("TileEntities/Coins/5A")},
        {CoinPattern.B5, Resources.Load<GameObject>("TileEntities/Coins/5B")},
        {CoinPattern.None, null }
    };

    public static Dictionary<CoinPattern, HashSet<FloorLocation>> COIN_LOCATIONS = new Dictionary<CoinPattern, HashSet<FloorLocation>>()
    {
        {CoinPattern.A1, new HashSet<FloorLocation>{ FloorLocation.Center } },
        {CoinPattern.A3, new HashSet<FloorLocation>{ FloorLocation.Center, FloorLocation.MidTop, FloorLocation.MidBottom }},
        {CoinPattern.B3, new HashSet<FloorLocation>{ FloorLocation.Center, FloorLocation.BottomLeft, FloorLocation.TopRight }},
        {CoinPattern.C3, new HashSet<FloorLocation>{ FloorLocation.Center, FloorLocation.TopLeft, FloorLocation.BottomRight }},
        {CoinPattern.D3, new HashSet<FloorLocation>{ FloorLocation.Center, FloorLocation.MidLeft, FloorLocation.MidRight }},
        {CoinPattern.A4, new HashSet<FloorLocation>{ FloorLocation.TopLeft, FloorLocation.TopRight, FloorLocation.BottomLeft, FloorLocation.BottomRight }},
        {CoinPattern.B4, new HashSet<FloorLocation>{ FloorLocation.MidTop, FloorLocation.MidBottom, FloorLocation.MidLeft, FloorLocation.MidRight }},
        {CoinPattern.A5, new HashSet<FloorLocation>{}},
        {CoinPattern.B5, new HashSet<FloorLocation>{ FloorLocation.Center, FloorLocation.TopLeft, FloorLocation.TopRight, FloorLocation.BottomLeft, FloorLocation.BottomRight }},
        {CoinPattern.None, new HashSet<FloorLocation>{} }
    };

    private static CoinPattern[] fiveCoins = { CoinPattern.A5, CoinPattern.B5 };
    private static CoinPattern[] fourCoins = { CoinPattern.A4, CoinPattern.B4 };
    private static CoinPattern[] threeCoins = { CoinPattern.A3, CoinPattern.B3, CoinPattern.C3, CoinPattern.D3 };
    private static CoinPattern[] oneCoin = { CoinPattern.A1 };

    public static Dictionary<int, CoinPattern[]> coinPrefabs = new Dictionary<int, CoinPattern[]>()
    {
        {5, fiveCoins},
        {4, fourCoins},
        {3, threeCoins},
        {1, oneCoin}
    };

    static GameObject storageOne = Resources.Load<GameObject>("TileEntities/Furniture/Storage_1");
    static GameObject storageTwo = Resources.Load<GameObject>("TileEntities/Furniture/Storage_2");
    static GameObject fountain = Resources.Load<GameObject>("TileEntities/Furniture/Fountain");
    static GameObject pillarsWhole = Resources.Load<GameObject>("TileEntities/Furniture/Pillars_Whole");
    static GameObject pillarsBrokenOne = Resources.Load<GameObject>("TileEntities/Furniture/Pillars_Broken_1");
    static GameObject pillarsBrokenTwo = Resources.Load<GameObject>("TileEntities/Furniture/Pillars_Broken_2");
    static GameObject mazeOne = Resources.Load<GameObject>("TileEntities/Furniture/Maze_1");
    static GameObject mazeTwo = Resources.Load<GameObject>("TileEntities/Furniture/Maze_2");
    static GameObject mazeThree = Resources.Load<GameObject>("TileEntities/Furniture/Maze_3");
    static GameObject mazeFour = Resources.Load<GameObject>("TileEntities/Furniture/Maze_4");
    static GameObject lavaOne = Resources.Load<GameObject>("TileEntities/Furniture/Lava_1");
    static GameObject lavaTwo = Resources.Load<GameObject>("TileEntities/Furniture/Lava_2");
    static GameObject dungeonOne = Resources.Load<GameObject>("TileEntities/Furniture/Dungeon_1");
    static GameObject dungeonTwo = Resources.Load<GameObject>("TileEntities/Furniture/Dungeon_2");

    public static HashSet<GameObject> allFurniture = new HashSet<GameObject>(){
        storageOne, storageTwo, fountain, pillarsWhole, pillarsBrokenOne, 
        pillarsBrokenTwo, mazeOne, mazeTwo, mazeThree, mazeFour,
        lavaOne, lavaTwo, dungeonOne, dungeonTwo
    };

    public static Dictionary<FloorLocation, HashSet<GameObject>> FURNITURE_LOCATIONS = new Dictionary<FloorLocation, HashSet<GameObject>>()
    {
        {FloorLocation.TopLeft, new HashSet<GameObject>{ pillarsWhole, pillarsBrokenOne, pillarsBrokenTwo, mazeTwo, mazeThree, mazeFour, lavaTwo, dungeonOne }},
        {FloorLocation.MidTop, new HashSet<GameObject>{ mazeOne, mazeTwo, mazeFour }},
        {FloorLocation.TopRight, new HashSet<GameObject>{ pillarsWhole, pillarsBrokenOne, mazeTwo, mazeThree, mazeFour, lavaOne, lavaTwo, dungeonOne }},
        {FloorLocation.MidLeft, new HashSet<GameObject>{ mazeOne, mazeThree, mazeFour }},
        {FloorLocation.Center, new HashSet<GameObject>{ fountain, mazeFour, lavaOne, lavaTwo }},
        {FloorLocation.MidRight, new HashSet<GameObject>{ mazeOne, mazeThree, mazeFour, lavaOne }},
        {FloorLocation.BottomLeft, new HashSet<GameObject>{ pillarsWhole, pillarsBrokenOne, mazeTwo, mazeThree, mazeFour, lavaOne, dungeonOne, dungeonTwo }},
        {FloorLocation.MidBottom, new HashSet<GameObject>{ mazeOne, mazeTwo, mazeFour }},
        {FloorLocation.BottomRight, new HashSet<GameObject>{ pillarsWhole, pillarsBrokenOne, pillarsBrokenTwo, mazeTwo, mazeThree, mazeFour, lavaTwo, dungeonOne }}
    };
}


