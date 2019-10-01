using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class World : MonoBehaviour
{
    public int generation_iteration_ = 10;
    private List<Room> room_list_;
    private List<Room> room_clone_list_ = new List<Room>();
    // Start is called before the first frame update
    void Start()
    {
        // get room prefabs from Assets/Prefabs/Rooms
        room_list_ = ReadTilemapsToRoom();
        //room_clone_list_.Add(new Room(Instantiate(room_list_[1].game_object_, GameObject.Find("Grid").transform)));
        //GameObject gameobject = Instantiate(room_list_[1].game_object_, GameObject.Find("Grid").transform);
        //GameObject gameobject2 = Instantiate(room_list_[0].game_object_, GameObject.Find("Grid").transform);
        //gameobject.transform.position = new Vector3(2,0,0);
        //gameobject.GetComponent<Tilemap>().tileAnchor = new Vector3(5.5f, 0.5f, 0);
        //gameobject2.GetComponent<Tilemap>().tileAnchor = new Vector3(-5.5f, 0.5f, 0);
        //room_clone_list_[0].game_object_.GetComponent<Tilemap>().tileAnchor = new Vector3(5.5f, 0.5f, 0);
        SpawnRooms();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    // returns indices rooms_list with doors facing orientation
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

    // return a list of indices of rooms that does not overlap all existing rooms
    public List<Vector2Int> FindRoomsThatFitDoor(Room.Door door, List<int> rooms, string orientation, ref List<Vector3Int> offsetlist)
    {
        if (offsetlist.Count > 0)
        {
            offsetlist.Clear();
        }
        List<Vector2Int> door_to_room_pair = new List<Vector2Int>();
        Vector3Int offset_holder = new Vector3Int(0, 0, 0);
        if (orientation == "NW")
        {
            foreach (int i in rooms)
            {
                for (int d = 0; d < room_list_[i].nw_doors_.Count; d++)
                {
                    offset_holder = room_list_[i].GetPositionOffset(room_list_[i].nw_doors_[d], door);
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
                        door_to_room_pair.Add(new Vector2Int(i, d));
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
                        door_to_room_pair.Add(new Vector2Int(i, d));
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
                        door_to_room_pair.Add(new Vector2Int(i, d));
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
                        door_to_room_pair.Add(new Vector2Int(i, d));
                        offsetlist.Add(offset_holder);
                    }
                }
            }
        }
        return door_to_room_pair;
    }

    //public void AddRoomToWorld(Vector2Int)
    //{
    //    room_clone_list_.Add(new Room(Instantiate(room_list_[1].game_object_, GameObject.Find("Grid").transform)));
    //}

    public void SpawnRooms()
    {
        // create origin room, selects random room from list of rooms
        room_clone_list_.Add(new Room(Instantiate(room_list_[0].game_object_, GameObject.Find("Grid").transform)));

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
                if (list_length > 0)
                {
                    int temp_rand = Random.Range(0, list_length);
                    room_clone_list_.Add(new Room(Instantiate(room_list_[temp_list[temp_rand].x].game_object_, GameObject.Find("Grid").transform)));
                    room_clone_list_[room_clone_list_.Count - 1].ConfigureRoomWithOffset(reference_list[temp_rand]);
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
                    room_clone_list_.Add(new Room(Instantiate(room_list_[temp_list[temp_rand].x].game_object_, GameObject.Find("Grid").transform)));
                    room_clone_list_[room_clone_list_.Count - 1].ConfigureRoomWithOffset(reference_list[temp_rand]);
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
                    room_clone_list_.Add(new Room(Instantiate(room_list_[temp_list[temp_rand].x].game_object_, GameObject.Find("Grid").transform)));
                    room_clone_list_[room_clone_list_.Count - 1].ConfigureRoomWithOffset(reference_list[temp_rand]);
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
                    room_clone_list_.Add(new Room(Instantiate(room_list_[temp_list[temp_rand].x].game_object_, GameObject.Find("Grid").transform)));
                    room_clone_list_[room_clone_list_.Count - 1].ConfigureRoomWithOffset(reference_list[temp_rand]);
                }
            }
        }
    }
}
