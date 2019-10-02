using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class World : MonoBehaviour
{
    // inverse dictionary
    public Dictionary<Room.TileType, string> TypeToSprite = new Dictionary<Room.TileType, string>()
    {
        { Room.TileType.CONNECTOR, "connector_block" },
        { Room.TileType.GRASS, "grass_block" },
        { Room.TileType.DIRT, "dirt_block" }
    };

    public int generation_iteration_ = 10;
    private List<Room> room_list_;
    private List<Room> room_clone_list_ = new List<Room>();
    private GameObject base_layer;
    // Start is called before the first frame update
    void Start()
    {
        // Assign BaseLayer Tilemap
        base_layer = GameObject.Find("Grid").transform.GetChild(0).gameObject;

        // Get all room prefabs starting with "Rm" from Assets/Prefabs/Rooms 
        // and returns a list of instantiated Rooms using the GameObjects
        room_list_ = ReadTilemapsToRoom();

        // Procedurally spawns a map by joining randomly joining the listed Rooms
        SpawnRooms();
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
        string[] rm_guids = AssetDatabase.FindAssets("Rm", new[] { "Assets/Prefabs/Rooms" });
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
        room_clone_list_.Add(new Room(room_list_[0].game_object_));
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
                SetTile(room.tile_list_[x, y].tile_type_, x + offset.x + room.position_offset_.x, y + offset.y + room.position_offset_.y);
            }
        }
    }

    public void SetTile(Room.TileType type, int x, int y)
    {
        if (type == Room.TileType.NONE)
        {
            return;
        }
        base_layer.GetComponent<Tilemap>().SetTile(new Vector3Int(x, y, 0), GeneralFunctions.GetTileBase(TypeToSprite[type]));
    } 
}
