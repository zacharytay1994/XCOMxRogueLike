using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateMap : MonoBehaviour
{
    // map width and height
    public int width_ = 10;
    public int height_ = 10;

    // TILEMAPS
    // ground base
    private int[,] ground_base_fa_;

    // Reference to WorldManager
    private WorldManager world_manager_;

    // Start is called before the first frame update
    void Start()
    {
        // Get get and assign grid manager component
        world_manager_ = gameObject.GetComponent<WorldManager>();
        ground_base_fa_ = GenerateArray(width_, height_, false);
        print("Array Generated");
        RenderMap(ground_base_fa_,
            GeneralFunctions.GetTilemap(gameObject, "Tilemap - Ground - Base"),
            GeneralFunctions.GetTileBase("dirt_block"));
        RenderMap(GenerateArrayRandom(width_/2, height_/ 2),
            GeneralFunctions.GetTilemap(gameObject, "Tilemap - 1st - Base"),
            GeneralFunctions.GetTileBase("grass_block"));
        print("Map Rendered");
    }

    public int[,] GenerateArray(int width, int height, bool isEmpty)
    {
        int[,] tempArray = new int[width, height];
        int flag = isEmpty ? 0 : 1;
        for (int x = 0; x <= tempArray.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= tempArray.GetUpperBound(1); y++)
            {
                tempArray[x, y] = flag;
            }
        }
        return tempArray;
    }

    // test function
    public int[,] GenerateArrayRandom(int width, int height)
    {
        int[,] tempArray = new int[width, height];
        for (int x = 0; x <= tempArray.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= tempArray.GetUpperBound(1); y++)
            {
                tempArray[x, y] = Random.Range(0,2);
            }
        }
        return tempArray;
    }

    public void RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
    {
        if (tilemap == null)
        {
            Debug.Log("Tilemap input is null");
            return;
        }
        if (tile == null)
        {
            Debug.Log("TileBase asset not found");
            return;
        }
        world_manager_.SetCurrentTilemapLayer(tilemap.name);
        tilemap.ClearAllTiles();
        Vector3Int tile_position = new Vector3Int(0, 0, 0);
        for (int x = 0; x <= map.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= map.GetUpperBound(1); y++)
            {
                if (map[x,y] == 1)
                {
                    tile_position.x = x;
                    tile_position.y = y;
                    tilemap.SetTile(tile_position, tile);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
