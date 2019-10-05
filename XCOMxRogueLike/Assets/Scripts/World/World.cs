using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class World : MonoBehaviour
{
    //// inverse dictionary
    //public Dictionary<Constants.TileType, string> TypeToSprite = new Dictionary<Room.TileType, string>()
    //{
    //    { Room.TileType.CONNECTOR, "connector_block" },
    //    { Room.TileType.GRASS, "grass_block" },
    //    { Room.TileType.DIRT, "dirt_block" }
    //};

    public int generation_iteration_ = 0;
    public int number_of_creepers_ = 10;
    private List<Room> room_list_;
    private List<Room> room_clone_list_ = new List<Room>();
    private GameObject base_layer_;
    private GameObject entity_layer_;
    private Tilemap base_tilemap_;
    private Tilemap entity_tilemap_;

    public AI pathfinding = new AI();


    // World grid variables
    private Constants.RoomTile[,] grid_base_layer_;
    private Constants.RoomTile[,] entity_base_layer_;
    private int[,] collision_flags_;

    // List of walkable areas for testing purpose
    public List<Vector2Int> walkable_cells_ = new List<Vector2Int>();
    public Vector2Int grid_offset_;

    // Start is called before the first frame update
    void Start()
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
        ProcessTiles();
        List<Node> path = new List<Node>();
        bool test = pathfinding.FindPath(new Vector2Int(0, 5), new Vector2Int(5, 10), ref path);
        LoadEntities();
    }

    void InitializeEntities()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject.Find("Entities").transform.GetChild(i).gameObject.GetComponent<EntityMove>().WorldReadyInit();
        }
    }

    void LoadEntities()
    {
        // get creeper prefab
        GameObject creeper_prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Entities/Creeper.prefab", typeof(GameObject));
        Transform parent_transform = GameObject.Find("Entities").transform;
        for (int i = 0; i < number_of_creepers_; i++)
        {
            Instantiate(creeper_prefab, parent_transform).GetComponent<EntityMove>().WorldReadyInit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Pre  : room_list_ is empty
    // Post : room_list_ is filled with rooms, if any
    public List<Room> ReadTilemapsToRoom()
    {
        List<Room> temp_list = new List<Room>();
        string[] rm_guids = AssetDatabase.FindAssets("CRm", new[] { "Assets/Prefabs/Rooms" });
        foreach (string s in rm_guids)
        {
            temp_list.Add(new Room((GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(GameObject))));
        }
        return temp_list;
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
                SetTile(room.tile_list_[x, y].tile_type_, room.tile_list_[x,y].top_tile_type_,
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

    // temporary - maybe - solution
    // Pre  : base_tilemap_ is filled
    // Post : grid_base_layer_ is filled with Room.RoomTiles
    public void ProcessTiles()
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
                    if (collision_flags_[x,y] != 1)
                    {
                        walkable_cells_.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        pathfinding.grid_.InitGrid(collision_flags_);
    }
}
