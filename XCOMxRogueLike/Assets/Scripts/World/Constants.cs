using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that contains all constants
// e.g. dictionaries
public class Constants : MonoBehaviour
{
    // ================================================== //
    // TYPES OF TILES - ADD NEW TILES HERE
    // ================================================== //
    // Stores tile data for game processes
    public struct RoomTile
    {
        public TileType tile_type_;             // the type of tile it is
        public bool is_occupied_;               // if the top of the tile is currently occupied
        public bool is_walkable_;               // if the tile is walkable
        public Vector2Int position_in_grid_;    // position in grid coordinates
        public Vector2Int position_in_world_;   // position in world coordinates

        public void Init(TileType type, Vector2Int gridposition)
        {
            if (void_tiles_.Contains(type))
            {
                is_occupied_ = false;
                is_walkable_ = false;
                position_in_grid_ = gridposition;
                tile_type_ = type;
            }
            else if (solid_tiles_.Contains(type))
            {
                is_occupied_ = false;
                is_walkable_ = true;
                tile_type_ = type;
                position_in_grid_ = gridposition;
                tile_type_ = type;
            }
        }
    }
    // add new enums here for new tiles
    public enum TileType
    {
        NONE,
        CONNECTOR,
        GRASS,
        DIRT,
    }
    // add new dictionary entries here for new tiles
    // dictionary of tiles, key : filename of asset, value : enum TileType
    public static Dictionary<string, TileType> SpriteToType = new Dictionary<string, TileType>()
    {
        { "connector_block", TileType.CONNECTOR },
        { "grass_block", TileType.GRASS },
        { "dirt_block", TileType.DIRT }
    };

    // inverse dictionary
    public static Dictionary<TileType, string> TypeToSprite = new Dictionary<TileType, string>()
    {
        { TileType.CONNECTOR, "connector_block" },
        { TileType.GRASS, "grass_block" },
        { TileType.DIRT, "dirt_block" }
    };

    // define if tiles type is solid or empty here here
    public static readonly TileType[] solid_hash_ = { TileType.CONNECTOR, TileType.GRASS, TileType.DIRT };
    public static readonly HashSet<TileType> solid_tiles_ = new HashSet<TileType>(solid_hash_);
    public static readonly TileType[] void_hash_ = { TileType.NONE };
    public static readonly HashSet<TileType> void_tiles_ = new HashSet<TileType>(void_hash_);

    public static TileType GetTypeFromSprite(string spritename)
    {
        return SpriteToType[spritename];
    }

    public static string GetSpriteFromType(TileType type)
    {
        return TypeToSprite[type];
    }
}
