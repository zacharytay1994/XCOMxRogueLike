using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityMove : MonoBehaviour
{
    private Vector3Int grid_initial_coordinate_ = new Vector3Int(0, 0, 0);
    private Grid grid_;
    World world_;

    // Movement Variables for pathfinding
    private     Vector2Int start_position_;         // path start in grid coordinates
    private     Vector2Int end_position_;           // path end in grid coordinates
    bool        executing_path_ = false;            // if a path is currently being executed
    List<Node>  current_path_;                       // the current path in terms of Node checkpoints
    float       frame_time_ = 60.0f;                // (temporary) number of frame updates per cycle
    float       counter_ = 0.0f;                    // (temporary) increment counter
    float       counter_speed_ = 5.0f;              // (temporary) speed to increment
    int         current_checkpoint_;                // (temporary) index, e.g. current_path_[current_checkpoint_]

    // world is initialized
    bool world_ready_ = false;

    // Gets and assigns reference to Grid object in scene tree
    // Sets entity transform component to world position equal to (0,0,0) grid position
    void Init()
    {
        grid_ = GameObject.Find("Grid").GetComponent<Grid>();
        gameObject.transform.position = grid_.GetCellCenterWorld(grid_initial_coordinate_);
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // Temporary spaghetti code to test pathfinding
        // Cycles and executes new path when old one ends
        if (world_ready_)
        {
            if (executing_path_)
            {
                if (current_checkpoint_ < current_path_.Count)
                {
                    if (counter_ < frame_time_)
                    {
                        counter_ += counter_speed_;
                    }
                    else
                    {
                        gameObject.transform.position = grid_.GetCellCenterWorld(new Vector3Int(current_path_[current_checkpoint_].gridX_ + world_.tilemap_.grid_offset_.x,
                            current_path_[current_checkpoint_].gridY_ + world_.tilemap_.grid_offset_.y, 0));
                        current_checkpoint_++;
                        counter_ = 0.0f;
                    }
                }
                else
                {
                    executing_path_ = false;
                }
            }
            else
            {
                GetNewRandomEnd(ref end_position_);
                if (GetPath(start_position_, end_position_, ref current_path_))
                {
                    executing_path_ = true;
                    current_checkpoint_ = 0;
                }
            }
        }
    }
    
    // Wrapper function that acesses Pathfinding object in world and calls function Pathfinding::FindPath()
    // Returns true if path is found
    // Pre  : World object initialized, Pathfinding object initialized
    // Post : Returns ref path if found and true, false if not
    bool GetPath(Vector2Int start, Vector2Int end, ref List<Node> path)
    {
        if (world_ != null)
        {
            return world_.pathfinding_.FindPath(start - world_.tilemap_.grid_offset_, end - world_.tilemap_.grid_offset_, ref path);
        }
        return false;
    }

    // Moves Entity in the grid by offsetting its grid position by a velocity component
    void Move(Vector3Int velocity)
    {
        grid_initial_coordinate_ += velocity;
        gameObject.transform.position = grid_.GetCellCenterWorld(grid_initial_coordinate_);
    }

    // -------------------------------------------------//
    // TENTATIVE FUNCTIONS - TEMPORARY
    // -------------------------------------------------//
    // Assigns world_ object reference and assigns random tile to start_ and end_position
    // called in object initialization phase
    void GetRandomStartEnd(ref Vector2Int start, ref Vector2Int end)
    {
        world_ = GameObject.Find("GameMain").GetComponent<World>();
        start = world_.tilemap_.walkable_cells_[Random.Range(0, world_.tilemap_.walkable_cells_.Count)] + world_.tilemap_.grid_offset_;
        end = world_.tilemap_.walkable_cells_[Random.Range(0, world_.tilemap_.walkable_cells_.Count)] + world_.tilemap_.grid_offset_;
    }

    // Assigns old end_position as new start_position, and randomly assigns new end_position
    void GetNewRandomEnd(ref Vector2Int end)
    {
        start_position_ = end_position_;
        end = world_.tilemap_.walkable_cells_[Random.Range(0, world_.tilemap_.walkable_cells_.Count)] + world_.tilemap_.grid_offset_;
    }

    // External initialization function called by World object
    // Pre  : Member variables uninitialized
    // Post : Member variables initialized
    // Reason : Some of these member variables depend on World object being initialized first, maybe not dunno lol
    public void WorldReadyInit()
    { 
        grid_ = GameObject.Find("Grid").GetComponent<Grid>();
        // Assigns a random walkable tile as the start_ and end_position_
        GetRandomStartEnd(ref start_position_, ref end_position_);
        // Flags world_ready_ as true to let relavant functions know that World object has been initialized
        // Relavant functions : Update() for now
        world_ready_ = true;
    }
}
