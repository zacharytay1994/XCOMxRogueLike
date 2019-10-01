using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum CellType
{
    None,
    Dirt,
    Grass
}

public struct CellData {
    // default constructor
    public CellData(bool solid)
    {
        isSolid_ = solid;
        isOccupied_ = false;
        cell_type_ = CellType.None;
    }

    public bool isSolid_;
    public bool isOccupied_;
    public CellType cell_type_;
}

public struct TilemapLayerData
{
    public string name_;
    public int height_;
    public int grid_offset_;
}

public class WorldManager : MonoBehaviour
{
    // Define grid variables
    // define 3d world grid
    private readonly int world_x_width_ = 100;    // 1-width of 3d-grid, going north-west
    private readonly int world_y_width_ = 100;    // 2-width of 3d-grid, going north-east
    private readonly int world_z_height_ = 5;      // height of 3d-grid, going north (upwards)

    private CellData[,,] grid_;

    // Tilemap layers, Key : tilemap name, Value : world grid height
    Dictionary<string, int> tilemap_layers_ = new Dictionary<string, int>()
    {
        { "Tilemap - Ground - Base", 0 },
        { "Tilemap - 1st - Base", 1 },
        { "Tilemap - 2nd - Base", 2 }
    };
    // world state
    private TilemapLayerData current_tilemap_layer_;


    // Start is called before the first frame update
    void Start()
    {
        InitWorldGrid();
    }

    // initialize world grid as empty cells
    void InitWorldGrid()
    {
        grid_ = new CellData[world_x_width_, world_y_width_, world_z_height_];
        for (int x = 0; x <= grid_.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= grid_.GetUpperBound(1); y++)
            {
                for (int z = 0; z <= grid_.GetUpperBound(2); z++)
                {
                    grid_[x, y, z] = new CellData(false);
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    CellData GetWorldCell(int x, int y, int z)
    {
        return grid_[x, y, z];
    }

    void SetWorldCell(int x, int y, int z, CellType celltype)
    {
        if (celltype != CellType.None)
        {
            grid_[x, y, z].isOccupied_ = true;
            grid_[x, y, z].isSolid_ = true;
        }
        grid_[x, y, z].cell_type_ = celltype;
    }

    public void SetCurrentTilemapLayer(string tilemapname)
    {
        current_tilemap_layer_.name_ = tilemapname;
        current_tilemap_layer_.height_ = tilemap_layers_[tilemapname];
        current_tilemap_layer_.grid_offset_ = tilemap_layers_[tilemapname];
    }
}
