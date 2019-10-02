using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy_Move : MonoBehaviour
{
    Rigidbody2D enemy_rigidbody_;
    private float speed_ = 10.0f;
    private Vector3Int grid_initial_coordinate_ = new Vector3Int(0, 0, -1);
    private Grid grid_;
    private enum tile
    {
        none,
        floor,
        wall
    }
    private List<tile> tile_list_;

    // Calculate the size of the grid
    void CalculateGrid()
    {
        Tilemap tilemap = grid_.transform.GetChild(0).GetComponent<Tilemap>();
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax - 1; x++)
        {
            for (int y = bounds.xMin; y < bounds.yMax - 1; y++)
            {
                string sprite_name = tilemap.GetTile<Tile>(new Vector3Int(x, y, 0)).sprite.name;
                switch (sprite_name)
                {
                    case "Test_Floor":
                        tile_list_.Add(tile.floor);
                        break;
                    
                    case "Test_Wall":
                        tile_list_.Add(tile.wall);
                        break;
                }
            }
        }
    }

    // Initialise Enemy Object
    void Init()
    {
        grid_ = GameObject.Find("TestGrid").GetComponent<Grid>();
        gameObject.transform.position = grid_.GetCellCenterWorld(grid_initial_coordinate_);
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
        //Move(new Vector3Int(1, 0, -1));
    }

    // Update is called once per frame
    void Update()
    {
        //enemy_rigidbody_.velocity = transform.right * speed_;
        //Move(new Vector3Int(1, 0, -1));

    }

    // Enemy move function
    void Move(Vector3Int velocity)
    {
        grid_initial_coordinate_ += velocity;
        gameObject.transform.position = grid_.GetCellCenterWorld(grid_initial_coordinate_);
    }
}
