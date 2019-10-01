using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room
{
    // ================================================== //
    // TYPES OF TILES - ADD NEW TILES HERE
    // ================================================== //
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
    public Dictionary<string, TileType> SpriteToType = new Dictionary<string, TileType>()
    {
        { "connector_block", TileType.CONNECTOR },
        { "grass_block", TileType.GRASS },
        { "dirt_block", TileType.DIRT }
    };
    // define if tiles type is solid or empty here here
    private static readonly TileType[] solid_hash_ = { TileType.CONNECTOR, TileType.GRASS, TileType.DIRT };
    private static readonly HashSet<TileType> solid_tiles_ = new HashSet<TileType>(solid_hash_);
    private static readonly TileType[] void_hash_ = { TileType.NONE };
    private static readonly HashSet<TileType> void_tiles_ = new HashSet<TileType>(void_hash_);

    // ================================================== //
    // TILE DEFINITION
    // ================================================== //
    public struct RoomTile
    {
        public TileType tile_type_;             // the type of tile it is
        public bool is_occupied_;               // if the top of the tile is currently occupied
        public bool is_edge_;                   // is the tile is on the edge of the room
        public bool is_walkable_;               // if the tile is walkable
        public Vector2Int position_in_room_;    // position offset from room center
        public Vector2Int position_in_world_;   // position offset from world center

        public void Init(TileType type, Vector2Int roomposition)
        {
            if (void_tiles_.Contains(type))
            {
                is_occupied_ = false;
                is_edge_ = false;
                is_walkable_ = false;
                position_in_room_ = roomposition;
                tile_type_ = type;
            }
            else if (solid_tiles_.Contains(type))
            {
                is_occupied_ = false;
                is_walkable_ = true;
                tile_type_ = type;
                position_in_room_ = roomposition;
                tile_type_ = type;
            }
        }
    }

    // ================================================== //
    // DOOR DEFINITION
    // ================================================== //
    public struct Door
    {
        public string orientation_; // face of door, i.e. north_west, north_east, south_west, south_east
        public Vector3Int grid_position_;

        public Door(string orientation, Vector3Int gridposition)
        {
            orientation_ = orientation;
            grid_position_ = gridposition;
        }
    }

    // ================================================== //
    // ROOM DEFINITION
    // ================================================== //
    public GameObject game_object_;     // game object representing the room
    public Tilemap tilemap_;            // tilemap it belongs to
    private int world_height_;          // value representing this rooms layer height in the world
    private int x_;                     // north_west dimension
    private int y_;                     // north_east dimension
    private Vector3Int room_origin_;    // world center of the room, bottom corner (0,0)
    private Vector3Int world_offset_;

    public RoomAABB bounding_;
    Vector3Int position_offset_ = new Vector3Int(0, 0, 0);
    public int layer_height_ = 0;

    // connector variables
    public int number_nw_doors;    // number of connectors the room hass
    public int number_ne_doors;
    public int number_sw_doors;
    public int number_se_doors;
    // door lists
    public List<Door> nw_doors_ = new List<Door>();
    public List<Door> ne_doors_ = new List<Door>();
    public List<Door> sw_doors_ = new List<Door>();
    public List<Door> se_doors_ = new List<Door>();

    RoomTile[,] tile_list_;         // tiles in room

    // room bounding box
    // ================================================== //
    // CONSTRUCTOR
    // ================================================== //
    public Room(GameObject gameobject)
    {
        InitRoom(gameobject);
    }

    // ================================================== //
    // INITIALIZATION FUNCTIONS
    // ================================================== //
    public void InitRoom(GameObject gameobject)
    {
        // setting tilemap variables
        game_object_ = gameobject;
        tilemap_ = game_object_.GetComponent<Tilemap>();
        // get used area of tilemap and define its dimensions
        tilemap_.CompressBounds();
        BoundsInt temp_bounds = tilemap_.cellBounds;
        x_ = temp_bounds.size.x;
        y_ = temp_bounds.size.y;
        room_origin_ = temp_bounds.min;
        tile_list_ = GetTiles(temp_bounds);
        bounding_ = new RoomAABB(temp_bounds.yMax - 1, temp_bounds.xMax - 1, temp_bounds.xMin, temp_bounds.yMin, layer_height_);
        world_height_ = 0;
    }

    // Fills tile list with tiles
    public RoomTile[,] GetTiles(BoundsInt bounds)
    {
        // create tile array
        RoomTile[,] tile_list = new RoomTile[x_,y_];
        TileType temp_tiletype;
        int xMaxMinusOne = bounds.xMax - 1;
        int yMaxMinusOne = bounds.yMax - 1;
        //int yMinPlusOne 
        // get all tiles within bounds
        for (int x = 0, rx = bounds.xMin; x < bounds.size.x; x++, rx++)
        {
            for (int y = 0, ry = bounds.yMin; y < bounds.size.y; y++, ry++)
            {
                if (tilemap_.GetTile<Tile>(new Vector3Int(rx,ry,0)) != null)
                {
                    temp_tiletype = SpriteToType[tilemap_.GetTile<Tile>(new Vector3Int(rx, ry, 0)).sprite.name];
                    // if tiletype is door and is valid (i.e. at convex edge of room), add to door list
                    if (temp_tiletype == TileType.CONNECTOR)
                    {
                        // if connector is a corner tile
                        if (rx == bounds.xMin && ry == bounds.yMin)
                        {
                            continue;
                        }
                        else if (rx == bounds.xMin && ry == yMaxMinusOne)
                        {
                            continue;
                        }
                        else if (ry == yMaxMinusOne && rx == xMaxMinusOne)
                        {
                            continue;
                        }
                        else if (rx == xMaxMinusOne && ry == bounds.yMin)
                        {
                            continue;
                        }
                        // if connector is an edge tile
                        else if (rx == bounds.xMin)
                        {
                            sw_doors_.Add(new Door("SW", new Vector3Int(rx, ry, layer_height_)));
                        }
                        else if (rx == xMaxMinusOne)
                        {
                            ne_doors_.Add(new Door("NE", new Vector3Int(rx, ry, layer_height_)));
                        }
                        else if (ry == bounds.yMin)
                        {
                            se_doors_.Add(new Door("SE", new Vector3Int(rx, ry, layer_height_)));
                        }
                        else if (ry == yMaxMinusOne)
                        {
                            nw_doors_.Add(new Door("NW", new Vector3Int(rx, ry, layer_height_)));
                        }
                    }
                    // gets sprite name and checks with dictionary to get enum TileType
                    tile_list[x, y].Init(temp_tiletype, new Vector2Int(x,y));
                }
                else
                {
                    tile_list[x, y].Init(TileType.NONE, new Vector2Int(x, y));
                }
            }
        }
        number_nw_doors = nw_doors_.Count;
        number_ne_doors = ne_doors_.Count;
        number_sw_doors = sw_doors_.Count;
        number_se_doors = se_doors_.Count;
        return tile_list;
    }
    // ================================================== //
    // ACTION FUNCTIONS
    // ================================================== //
    // positions room in world space door to door, 1 for up 0 for down
    public Vector3Int GetPositionOffset(Door thisdoor, Door otherdoor)
    {
        Vector3Int new_position = new Vector3Int();
        if (thisdoor.orientation_ == "NW")
        {
            new_position = new Vector3Int(otherdoor.grid_position_.x, otherdoor.grid_position_.y - 1, otherdoor.grid_position_.z - 2);
        }
        else if (thisdoor.orientation_ == "NE")
        {
            new_position = new Vector3Int(otherdoor.grid_position_.x - 1, otherdoor.grid_position_.y, otherdoor.grid_position_.z - 2);
        }
        else if (thisdoor.orientation_ == "SW")
        {
            new_position = new Vector3Int(otherdoor.grid_position_.x + 1, otherdoor.grid_position_.y, otherdoor.grid_position_.z + 2);
        }
        else if (thisdoor.orientation_ == "SE")
        {
            new_position = new Vector3Int(otherdoor.grid_position_.x, otherdoor.grid_position_.y + 1, otherdoor.grid_position_.z + 2);
        }
        return new_position - thisdoor.grid_position_;
    }

    public RoomAABB GetOffsetBounding(Vector3Int offset)
    {
        return new RoomAABB(bounding_.nw_bound_ + offset.y,
            bounding_.ne_bound_ + offset.x,
            bounding_.sw_bound_ + offset.x,
            bounding_.se_bound_ + offset.y,
            offset.z);
    }
    // sync relevant data to position offset, e.g. bounding, tilemap tile anchor
    public void ConfigureRoomWithOffset(Vector3Int offset)
    {
        bounding_ = new RoomAABB(bounding_.nw_bound_ + offset.y,
            bounding_.ne_bound_ + offset.x,
            bounding_.sw_bound_ + offset.x,
            bounding_.se_bound_ + offset.y,
            offset.z);
        game_object_.GetComponent<Tilemap>().tileAnchor = new Vector3(offset.x, offset.y, offset.z);
        for (int nw = 0; nw < nw_doors_.Count; nw++)
        {
            nw_doors_[nw] = new Door(nw_doors_[nw].orientation_, nw_doors_[nw].grid_position_ + offset);
        }
        for (int ne = 0; ne < ne_doors_.Count; ne++)
        {
            ne_doors_[ne] = new Door(ne_doors_[ne].orientation_, ne_doors_[ne].grid_position_ + offset);
        }
        for (int sw = 0; sw < sw_doors_.Count; sw++)
        {
            sw_doors_[sw] = new Door(sw_doors_[sw].orientation_, sw_doors_[sw].grid_position_ + offset);
        }
        for (int se = 0; se < se_doors_.Count; se++)
        {
            se_doors_[se] = new Door(se_doors_[se].orientation_, se_doors_[se].grid_position_ + offset);
        }
        world_height_ = offset.z / 2;
        game_object_.GetComponent<TilemapRenderer>().sortingOrder = world_height_; 
    }
}

// Simple bounding box for checking room intersection
public class RoomAABB
{
    // bounds
    public int nw_bound_;  // north west tilemap bounds in grid cells
    public int ne_bound_;  // north east tilemap bounds in grid cells
    public int sw_bound_;  // south west tilemap bounds in grid cells
    public int se_bound_;  // south east tilemap bounds in grid cells
    public int top_bound_;
    public int bottom_bound_;

    public RoomAABB(int nw, int ne, int sw, int se, int height)
    {
        nw_bound_ = nw;
        ne_bound_ = ne;
        sw_bound_ = sw;
        se_bound_ = se;
        top_bound_ = height + 2;
        bottom_bound_ = height - 2;
    }

    public void TranslateAABB(int x, int y, int z)
    {
        ne_bound_ += x;
        sw_bound_ += x;
        nw_bound_ += y;
        se_bound_ += y;
        top_bound_ += z;
        bottom_bound_ += z;
    }

    public bool IsOverlap(RoomAABB room)
    {
        if (room.ne_bound_ < sw_bound_ ||
            room.nw_bound_ < se_bound_ ||
            room.se_bound_ > nw_bound_ ||
            room.sw_bound_ > ne_bound_ ||
            room.top_bound_ < bottom_bound_ ||
            room.bottom_bound_ > top_bound_)
        {
            return false;
        }
        return true;
    }
}