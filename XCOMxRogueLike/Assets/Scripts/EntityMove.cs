using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityMove : MonoBehaviour
{
    private Vector3Int grid_initial_coordinate_ = new Vector3Int(0, 0, 0);
    private Grid grid_;
    World world_;

    // movement variables
    private Vector2Int start_position_;
    private Vector2Int end_position_;
    bool executing_path_ = false;
    float checkpoint_tolerance_ = 0.05f;
    List<Node> current_path;
    float frame_time_ = 60.0f;
    float counter_ = 0.0f;
    float counter_speed_ = 5.0f;
    int current_checkpoint_;

    // world is initialized
    bool world_ready_ = false;

    // Initialise Enemy Object
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
        // temp counter things to test
        if (world_ready_)
        {
            if (executing_path_)
            {
                if (current_checkpoint_ < current_path.Count)
                {
                    if (counter_ < frame_time_)
                    {
                        counter_ += counter_speed_;
                    }
                    else
                    {
                        gameObject.transform.position = grid_.GetCellCenterWorld(new Vector3Int(current_path[current_checkpoint_].gridX_ + world_.grid_offset_.x,
                            current_path[current_checkpoint_].gridY_ + world_.grid_offset_.y, 0));
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
                if (GetPath(start_position_, end_position_, ref current_path))
                {
                    executing_path_ = true;
                    current_checkpoint_ = 0;
                }
            }
        }
    }
    
    bool GetPath(Vector2Int start, Vector2Int end, ref List<Node> path)
    {
        if (world_ != null)
        {
            return world_.pathfinding.FindPath(start - world_.grid_offset_, end - world_.grid_offset_, ref path);
        }
        return false;
    }

    // Enemy move function
    void Move(Vector3Int velocity)
    {
        grid_initial_coordinate_ += velocity;
        gameObject.transform.position = grid_.GetCellCenterWorld(grid_initial_coordinate_);
    }

    // Tentative functions
    void GetRandomStartEnd(ref Vector2Int start, ref Vector2Int end)
    {
        world_ = GameObject.Find("GameMain").GetComponent<World>();
        start = world_.walkable_cells_[Random.Range(0, world_.walkable_cells_.Count)] + world_.grid_offset_;
        end = world_.walkable_cells_[Random.Range(0, world_.walkable_cells_.Count)] + world_.grid_offset_;
    }

    void GetNewRandomEnd(ref Vector2Int end)
    {
        start_position_ = end_position_;
        end = world_.walkable_cells_[Random.Range(0, world_.walkable_cells_.Count)] + world_.grid_offset_;
    }

    public void WorldReadyInit()
    { 
        grid_ = GameObject.Find("Grid").GetComponent<Grid>();
        GetRandomStartEnd(ref start_position_, ref end_position_);
        //Move((Vector3Int)end_position_);
        world_ready_ = true;
    }
}
