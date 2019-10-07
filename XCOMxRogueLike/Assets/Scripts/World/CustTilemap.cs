using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class CustTilemap
{
    public int generation_iteration_ = 20;
    private List<Room> room_list_;
    private List<Room> room_clone_list_ = new List<Room>();
    private GameObject base_layer_;
    private GameObject entity_layer_;
    private Tilemap base_tilemap_;
    private Tilemap entity_tilemap_;

    // World grid variables
    private Constants.RoomTile[,] grid_base_layer_;
    private Constants.RoomTile[,] entity_base_layer_;
    private int[,] collision_flags_;

    // List of walkable areas for testing purpose
    public List<Vector2Int> walkable_cells_ = new List<Vector2Int>();
    public Vector2Int grid_offset_;

    public CustTilemap()
    {}

    public void WorldInit(Pathfinding pathfinding)
    {
        // Assign BaseLayer Tilemap
        base_layer_ = GameObject.Find("Grid").transform.GetChild(0).gameObject;
        base_tilemap_ = base_layer_.GetComponent<Tilemap>();
        entity_layer_ = GameObject.Find("Grid").transform.GetChild(1).gameObject;
        entity_tilemap_ = entity_layer_.GetComponent<Tilemap>();

        // Get all room prefabs starting with "Rm" from Assets/Prefabs/Rooms 
        // and returns a list of instantiated Rooms using the GameObjects
        room_list_ = ReadTilemapsToRoom();

        // Procedurally spawns a map by joining randomly joining the listed Rooms
        SpawnRooms();

        // Process and generate grid of base tiles
        ProcessTiles(pathfinding);
    }
    // Pre  : room_list_ is empty
    // Post : room_list_ is filled with rooms, if any
    public static List<Room> ReadTilemapsToRoom()
    {
        List<Room> temp_list = new List<Room>();
        string[] rm_guids = AssetDatabase.FindAssets("CRm", new[] { "Assets/Prefabs/Rooms" });
        foreach (string s in rm_guids)
        {
            temp_list.Add(new Room((GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(GameObject))));
        }
        return temp_list;
    }

    // Pre  : filled rooms_list_ with Room objects
    // Post : randomly conjoined Rooms instantiated under Grid object
    public void SpawnRooms()
    {
        // create origin room, selects room 0, at least one room should be present as a prefab
        room_clone_list_.Add(new Room(room_list_[3].game_object_));
        FillBaseTilemap(room_clone_list_[0], new Vector3Int(0, 0, 0));

        for (int i = 0; i < generation_iteration_; i++)
        {
            if (i > room_clone_list_.Count - 1)
            {
                break;
            }
            // append rooms to origin room
            foreach (Room.Door d in room_clone_list_[i].nw_doors_)
            {
                List<Vector3Int> reference_list = new List<Vector3Int>();
                List<Vector2Int> temp_list = FindRoomsThatFitDoor(d, FindRoomsWithFacingDoor("SE"), "SE", ref reference_list);
                int list_length = temp_list.Count;
                // if one or more possible conjoining room combinations is found, randomly select a combination to append to existing room
                if (list_length > 0)
                {
                    int temp_rand = Random.Range(0, list_length);
                    room_clone_list_.Add(new Room(room_list_[temp_list[temp_rand].x].game_object_));
                    room_clone_list_[room_clone_list_.Count - 1].ConfigureRoomWithOffset(reference_list[temp_rand]);
                    FillBaseTilemap(room_list_[temp_list[temp_rand].x], reference_list[temp_rand]);
                }
            }
            foreach (Room.Door d in room_clone_list_[i].ne_doors_)
            {
                List<Vector3Int> reference_list = new List<Vector3Int>();
                List<Vector2Int> temp_list = FindRoomsThatFitDoor(d, FindRoomsWithFacingDoor("SW"), "SW", ref reference_list);
                int list_length = temp_list.Count;
                if (list_length > 0)
                {
                    int temp_rand = Random.Range(0, list_length);
                    room_clone_list_.Add(new Room(room_list_[temp_list[temp_rand].x].game_object_));
                    room_clone_list_[room_clone_list_.Count - 1].ConfigureRoomWithOffset(reference_list[temp_rand]);
                    FillBaseTilemap(room_list_[temp_list[temp_rand].x], reference_list[temp_rand]);
                }
            }
            foreach (Room.Door d in room_clone_list_[i].sw_doors_)
            {
                List<Vector3Int> reference_list = new List<Vector3Int>();
                List<Vector2Int> temp_list = FindRoomsThatFitDoor(d, FindRoomsWithFacingDoor("NE"), "NE", ref reference_list);
                int list_length = temp_list.Count;
                if (list_length > 0)
                {
                    int temp_rand = Random.Range(0, list_length);
                    room_clone_list_.Add(new Room(room_list_[temp_list[temp_rand].x].game_object_));
                    room_clone_list_[room_clone_list_.Count - 1].ConfigureRoomWithOffset(reference_list[temp_rand]);
                    FillBaseTilemap(room_list_[temp_list[temp_rand].x], reference_list[temp_rand]);
                }
            }
            foreach (Room.Door d in room_clone_list_[i].se_doors_)
            {
                List<Vector3Int> reference_list = new List<Vector3Int>();
                List<Vector2Int> temp_list = FindRoomsThatFitDoor(d, FindRoomsWithFacingDoor("NW"), "NW", ref reference_list);
                int list_length = temp_list.Count;
                if (list_length > 0)
                {
                    int temp_rand = Random.Range(0, list_length);
                    room_clone_list_.Add(new Room(room_list_[temp_list[temp_rand].x].game_object_));
                    room_clone_list_[room_clone_list_.Count - 1].ConfigureRoomWithOffset(reference_list[temp_rand]);
                    FillBaseTilemap(room_list_[temp_list[temp_rand].x], reference_list[temp_rand]);
                }
            }
        }
        room_clone_list_.Clear();
    }

    public void FillBaseTilemap(Room room, Vector3Int offset)
    {
        for (int y = 0; y < room.tile_list_.GetLength(1); y++)
        {
            for (int x = 0; x < room.tile_list_.GetLength(0); x++)
            {
                SetTile(room.tile_list_[x, y].tile_type_, room.tile_list_[x, y].top_tile_type_,
                    x + offset.x + room.position_offset_.x, y + offset.y + room.position_offset_.y);
            }
        }
    }

    public void SetTile(Constants.TileType type, Constants.TileType toptype, int x, int y)
    {
        if (type == Constants.TileType.NONE)
        {
            return;
        }
        base_tilemap_.SetTile(new Vector3Int(x, y, 0), GeneralFunctions.GetTileBase(Constants.GetSpriteFromType(type)));
        if (toptype != Constants.TileType.NONE)
        {
            entity_tilemap_.SetTile(new Vector3Int(x, y, 0), GeneralFunctions.GetTileBase(Constants.GetSpriteFromType(toptype)));
        }
    }

    // Returns a list of integers that represents Room Indices in room_list_ with valid facing doors
    // e.g. if only room_list_[0] and room_list_[2] has NW facing doors,
    // FindRoomsWithFacingDoor("NW") returns {0,2}
    public List<int> FindRoomsWithFacingDoor(string orientation)
    {
        List<int> temp_list = new List<int>();
        if (orientation == "NW")
        {
            for (int i = 0; i < room_list_.Count; i++)
            {
                if (room_list_[i].number_nw_doors > 0)
                {
                    temp_list.Add(i);
                }
            }
        }
        else if (orientation == "NE")
        {
            for (int i = 0; i < room_list_.Count; i++)
            {
                if (room_list_[i].number_ne_doors > 0)
                {
                    temp_list.Add(i);
                }
            }
        }
        else if (orientation == "SW")
        {
            for (int i = 0; i < room_list_.Count; i++)
            {
                if (room_list_[i].number_sw_doors > 0)
                {
                    temp_list.Add(i);
                }
            }
        }
        else if (orientation == "SE")
        {
            for (int i = 0; i < room_list_.Count; i++)
            {
                if (room_list_[i].number_se_doors > 0)
                {
                    temp_list.Add(i);
                }
            }
        }
        return temp_list;
    }

    // Returns a list of Room-Door pairs that fit a target door, and get the offset required to transform
    // the position of the prefab clone to designated position
    // Pre  : offsetlist empty
    // Post : offsetlist contains all offsets of room-door pairs
    public List<Vector2Int> FindRoomsThatFitDoor(Room.Door door, List<int> rooms, string orientation, ref List<Vector3Int> offsetlist)
    {
        if (offsetlist.Count > 0)
        {
            offsetlist.Clear();
        }
        List<Vector2Int> room_to_door_pair = new List<Vector2Int>();
        Vector3Int offset_holder = new Vector3Int(0, 0, 0);
        if (orientation == "NW")
        {
            foreach (int i in rooms)
            {
                for (int d = 0; d < room_list_[i].nw_doors_.Count; d++)
                {
                    // Get the offset of the room to position room.nw_doors_[d] to door
                    offset_holder = room_list_[i].GetPositionOffset(room_list_[i].nw_doors_[d], door);
                    // Get the temporary bounding box used in calculation with the offset
                    RoomAABB temp_aabb = room_list_[i].GetOffsetBounding(offset_holder);
                    bool temp_flag = false;
                    // Test if repositioned bounding box intersects with any existing room,
                    // if not, add it to list to be considered a valid room-door combination
                    foreach (Room r in room_clone_list_)
                    {
                        if (r.bounding_.IsOverlap(temp_aabb))
                        {
                            temp_flag = true;
                        }
                    }
                    if (!temp_flag)
                    {
                        room_to_door_pair.Add(new Vector2Int(i, d));
                        offsetlist.Add(offset_holder);
                    }
                }
            }
        }
        else if (orientation == "NE")
        {
            foreach (int i in rooms)
            {
                for (int d = 0; d < room_list_[i].ne_doors_.Count; d++)
                {
                    offset_holder = room_list_[i].GetPositionOffset(room_list_[i].ne_doors_[d], door);
                    RoomAABB temp_aabb = room_list_[i].GetOffsetBounding(offset_holder);
                    bool temp_flag = false;
                    foreach (Room r in room_clone_list_)
                    {
                        if (r.bounding_.IsOverlap(temp_aabb))
                        {
                            temp_flag = true;
                        }
                    }
                    if (!temp_flag)
                    {
                        room_to_door_pair.Add(new Vector2Int(i, d));
                        offsetlist.Add(offset_holder);
                    }
                }
            }
        }
        else if (orientation == "SW")
        {
            foreach (int i in rooms)
            {
                for (int d = 0; d < room_list_[i].sw_doors_.Count; d++)
                {
                    offset_holder = room_list_[i].GetPositionOffset(room_list_[i].sw_doors_[d], door);
                    RoomAABB temp_aabb = room_list_[i].GetOffsetBounding(offset_holder);
                    bool temp_flag = false;
                    foreach (Room r in room_clone_list_)
                    {
                        if (r.bounding_.IsOverlap(temp_aabb))
                        {
                            temp_flag = true;
                        }
                    }
                    if (!temp_flag)
                    {
                        room_to_door_pair.Add(new Vector2Int(i, d));
                        offsetlist.Add(offset_holder);
                    }
                }
            }
        }
        else if (orientation == "SE")
        {
            foreach (int i in rooms)
            {
                for (int d = 0; d < room_list_[i].se_doors_.Count; d++)
                {
                    offset_holder = room_list_[i].GetPositionOffset(room_list_[i].se_doors_[d], door);
                    RoomAABB temp_aabb = room_list_[i].GetOffsetBounding(offset_holder);
                    bool temp_flag = false;
                    foreach (Room r in room_clone_list_)
                    {
                        if (r.bounding_.IsOverlap(temp_aabb))
                        {
                            temp_flag = true;
                        }
                    }
                    if (!temp_flag)
                    {
                        room_to_door_pair.Add(new Vector2Int(i, d));
                        offsetlist.Add(offset_holder);
                    }
                }
            }
        }
        return room_to_door_pair;
    }

    // temporary - maybe - solution
    // Pre  : base_tilemap_ is filled
    // Post : grid_base_layer_ is filled with Room.RoomTiles
    public void ProcessTiles(Pathfinding pathfinding)
    {
        // get base tilemap bounds
        base_tilemap_.CompressBounds();
        BoundsInt bounds = base_tilemap_.cellBounds;
        grid_base_layer_ = new Constants.RoomTile[bounds.size.x, bounds.size.y];
        entity_base_layer_ = new Constants.RoomTile[bounds.size.x, bounds.size.y];
        collision_flags_ = new int[bounds.size.x, bounds.size.y];
        grid_offset_ = new Vector2Int(bounds.xMin, bounds.yMin);
        // loop through and init all tiles within bounds
        for (int y = 0, ry = bounds.yMin; ry < bounds.yMax - 1; y++, ry++)
        {
            for (int x = 0, rx = bounds.xMin; rx < bounds.xMax - 1; x++, rx++)
            {
                Vector3Int grid_position = new Vector3Int(rx, ry, 0);

                if (base_tilemap_.GetTile<Tile>(grid_position) != null)
                {
                    grid_base_layer_[x, y].Init(Constants.GetTypeFromSprite(GeneralFunctions.GetTileSpriteName(base_tilemap_, grid_position)),
                        (Vector2Int)grid_position);
                }
                else
                {
                    grid_base_layer_[x, y].Init(Constants.TileType.NONE, (Vector2Int)grid_position);
                    collision_flags_[x, y] = 1;
                }

                if (entity_tilemap_.GetTile<Tile>(grid_position) != null)
                {
                    entity_base_layer_[x, y].Init(Constants.GetTypeFromSprite(GeneralFunctions.GetTileSpriteName(entity_tilemap_, grid_position)),
                        (Vector2Int)grid_position);
                    collision_flags_[x, y] = 1;
                }
                else
                {
                    entity_base_layer_[x, y].Init(Constants.TileType.NONE, (Vector2Int)grid_position);
                    // walkable cells for testing purposes
                    if (collision_flags_[x, y] != 1)
                    {
                        walkable_cells_.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        pathfinding.grid_.InitGrid(collision_flags_);
    }
}

public class Room
{
    // ================================================== //
    // LITE TILE DEFINITION - Only used for preprocessing
    // ================================================== //
    public struct LiteRoomTile
    {
        public Constants.TileType tile_type_;   // the type of tile it is
        public Constants.TileType top_tile_type_;

        public void Init(Constants.TileType type, Constants.TileType toptype)
        {
            tile_type_ = type;
            top_tile_type_ = toptype;
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
    public Tilemap entity_tilemap;
    private int world_height_;          // value representing this rooms layer height in the world
    private int x_;                     // north_west dimension
    private int y_;                     // north_east dimension
    private Vector3Int room_origin_;    // world center of the room, bottom corner (0,0)
    private Vector3Int world_offset_;   // offset of room world position

    public RoomAABB bounding_;
    public Vector3Int position_offset_ = new Vector3Int(0, 0, 0);
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

    public LiteRoomTile[,] tile_list_;         // tiles in room

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
        tilemap_ = game_object_.transform.GetChild(0).gameObject.GetComponent<Tilemap>();
        entity_tilemap = game_object_.transform.GetChild(1).gameObject.GetComponent<Tilemap>();
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
    public LiteRoomTile[,] GetTiles(BoundsInt bounds)
    {
        // create tile array
        LiteRoomTile[,] tile_list = new LiteRoomTile[x_, y_];
        Constants.TileType temp_tiletype;
        Constants.TileType top_tiletype;
        int xMaxMinusOne = bounds.xMax - 1;
        int yMaxMinusOne = bounds.yMax - 1;
        position_offset_ = new Vector3Int(bounds.xMin, bounds.yMin, 0);
        //int yMinPlusOne 
        // get all tiles within bounds
        for (int x = 0, rx = bounds.xMin; x < bounds.size.x; x++, rx++)
        {
            for (int y = 0, ry = bounds.yMin; y < bounds.size.y; y++, ry++)
            {
                Vector3Int grid_position = new Vector3Int(rx, ry, 0);
                if (tilemap_.GetTile<Tile>(grid_position) != null)
                {
                    temp_tiletype = Constants.GetTypeFromSprite(GeneralFunctions.GetTileSpriteName(tilemap_, grid_position));
                    if (entity_tilemap.GetTile<Tile>(grid_position) != null)
                    {
                        top_tiletype = Constants.GetTypeFromSprite(GeneralFunctions.GetTileSpriteName(entity_tilemap, grid_position));
                    }
                    else
                    {
                        top_tiletype = Constants.TileType.NONE;
                    }
                    // if tiletype is door and is valid (i.e. at convex edge of room), add to door list
                    if (temp_tiletype == Constants.TileType.CONNECTOR)
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
                    tile_list[x, y].Init(temp_tiletype, top_tiletype);
                }
                else
                {
                    tile_list[x, y].Init(Constants.TileType.NONE, Constants.TileType.NONE);
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
            new_position = new Vector3Int(otherdoor.grid_position_.x, otherdoor.grid_position_.y - 1, otherdoor.grid_position_.z);
        }
        else if (thisdoor.orientation_ == "NE")
        {
            new_position = new Vector3Int(otherdoor.grid_position_.x - 1, otherdoor.grid_position_.y, otherdoor.grid_position_.z);
        }
        else if (thisdoor.orientation_ == "SW")
        {
            new_position = new Vector3Int(otherdoor.grid_position_.x + 1, otherdoor.grid_position_.y, otherdoor.grid_position_.z);
        }
        else if (thisdoor.orientation_ == "SE")
        {
            new_position = new Vector3Int(otherdoor.grid_position_.x, otherdoor.grid_position_.y + 1, otherdoor.grid_position_.z);
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
        tilemap_.tileAnchor = new Vector3(offset.x, offset.y, offset.z);
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
        game_object_.transform.GetChild(0).gameObject.GetComponent<TilemapRenderer>().sortingOrder = world_height_;
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
