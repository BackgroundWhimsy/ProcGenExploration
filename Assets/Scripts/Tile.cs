using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Null, Up, Down, Left, Right
}

public enum TileType
{
    None, A1, B1, C1, D1, A2, B2, C2, D2, E2, F2, A3, B3, C3, D3, A4
}

public class Tile
{
    public TileType type;
    public bool up;
    public bool down;
    public bool left;
    public bool right;

    public Tile()
    {
        type = TileType.None;
        up = false;
        down = false;
        left = false;
        right = false;
    }

    public Tile(TileType type, bool up, bool down, bool left, bool right)
    {
        this.type = type;
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
    }

    public override string ToString()
    {
        return TileTypeToString(type);
    }

    public bool Equals(Tile other)
    {
        if (other == null)
        {
            return false;
        }

        return this.type == other.type;
    }

    public static float WIDTH = 16f;

    public static Tile NONE = new Tile(TileType.None, false, false, false, false);

    public static Tile A1 = new Tile(TileType.A1, true, false, false, false);
    public static Tile B1 = new Tile(TileType.B1, false, true, false, false);
    public static Tile C1 = new Tile(TileType.C1, false, false, true, false);
    public static Tile D1 = new Tile(TileType.D1, false, false, false, true);

    public static Tile A2 = new Tile(TileType.A2, false, false, true, true);
    public static Tile B2 = new Tile(TileType.B2, true, true, false, false);
    public static Tile C2 = new Tile(TileType.C2, true, false, true, false);
    public static Tile D2 = new Tile(TileType.D2, false, true, true, false);
    public static Tile E2 = new Tile(TileType.E2, true, false, false, true);
    public static Tile F2 = new Tile(TileType.F2, false, true, false, true);

    public static Tile A3 = new Tile(TileType.A3, false, true, true, true);
    public static Tile B3 = new Tile(TileType.B3, true, false, true, true);
    public static Tile C3 = new Tile(TileType.C3, true, true, false, true);
    public static Tile D3 = new Tile(TileType.D3, true, true, true, false);

    public static Tile A4 = new Tile(TileType.A4, true, true, true, true);

    public static Tile[] ALL = { A1, B1, C1, D1, A2, B2, C2, D2, E2, F2, A3, B3, C3, D3, A4 };
    public static Tile[] UP_TILES = { A1, B2, C2, E2, B3, C3, D3, A4 };
    public static Tile[] DOWN_TILES = { B1, B2, D2, F2, A3, C3, D3, A4 };
    public static Tile[] LEFT_TILES = { C1, A2, C2, D2, A3, B3, D3, A4 };
    public static Tile[] RIGHT_TILES = { D1, A2, E2, F2, A3, B3, C3, A4 };

    public static Dictionary<TileType, GameObject> TILE_PREFABS = new Dictionary<TileType, GameObject>()
    {   
        {TileType.A1, Resources.Load<GameObject>("Tiles/1A")},
        {TileType.B1, Resources.Load<GameObject>("Tiles/1B")},
        {TileType.C1, Resources.Load<GameObject>("Tiles/1C")},
        {TileType.D1, Resources.Load<GameObject>("Tiles/1D")},
        {TileType.A2, Resources.Load<GameObject>("Tiles/2A")},
        {TileType.B2, Resources.Load<GameObject>("Tiles/2B")},
        {TileType.C2, Resources.Load<GameObject>("Tiles/2C")},
        {TileType.D2, Resources.Load<GameObject>("Tiles/2D")},
        {TileType.E2, Resources.Load<GameObject>("Tiles/2E")},
        {TileType.F2, Resources.Load<GameObject>("Tiles/2F")},
        {TileType.A3, Resources.Load<GameObject>("Tiles/3A")},
        {TileType.B3, Resources.Load<GameObject>("Tiles/3B")},
        {TileType.C3, Resources.Load<GameObject>("Tiles/3C")},
        {TileType.D3, Resources.Load<GameObject>("Tiles/3D")},
        {TileType.A4, Resources.Load<GameObject>("Tiles/4A")},
        {TileType.None, null }
    };

    public static Dictionary<TileType, Sprite> TILE_SPRITES = new Dictionary<TileType, Sprite>()
    {
        {TileType.A1, Resources.Load<Sprite>("TileSprites/1A")},
        {TileType.B1, Resources.Load<Sprite>("TileSprites/1B")},
        {TileType.C1, Resources.Load<Sprite>("TileSprites/1C")},
        {TileType.D1, Resources.Load<Sprite>("TileSprites/1D")},
        {TileType.A2, Resources.Load<Sprite>("TileSprites/2A")},
        {TileType.B2, Resources.Load<Sprite>("TileSprites/2B")},
        {TileType.C2, Resources.Load<Sprite>("TileSprites/2C")},
        {TileType.D2, Resources.Load<Sprite>("TileSprites/2D")},
        {TileType.E2, Resources.Load<Sprite>("TileSprites/2E")},
        {TileType.F2, Resources.Load<Sprite>("TileSprites/2F")},
        {TileType.A3, Resources.Load<Sprite>("TileSprites/3A")},
        {TileType.B3, Resources.Load<Sprite>("TileSprites/3B")},
        {TileType.C3, Resources.Load<Sprite>("TileSprites/3C")},
        {TileType.D3, Resources.Load<Sprite>("TileSprites/3D")},
        {TileType.A4, Resources.Load<Sprite>("TileSprites/4A")},
        {TileType.None, Resources.Load<Sprite>("TileSprites/4A")}
    };

    public static string TileTypeToString(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.A1:
                return "A1";
            case TileType.B1:
                return "B1";
            case TileType.C1:
                return "C1";
            case TileType.D1:
                return "D1";
            case TileType.A2:
                return "A2";
            case TileType.B2:
                return "B2";
            case TileType.C2:
                return "C2";
            case TileType.D2:
                return "D2";
            case TileType.E2:
                return "E2";
            case TileType.F2:
                return "F2";
            case TileType.A3:
                return "A3";
            case TileType.B3:
                return "B3";
            case TileType.C3:
                return "C3";
            case TileType.D3:
                return "D3";
            case TileType.A4:
                return "A4";
            default:
                return "  ";
        }
    }

}


