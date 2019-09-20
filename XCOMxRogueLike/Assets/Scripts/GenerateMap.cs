using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateMap : MonoBehaviour
{
    // map width and height
    public int width = 100;
    public int height = 100;

    // TILEMAPS
    // ground base
    int[,] ground_base_fa_;

    // Start is called before the first frame update
    void Start()
    {
        ground_base_fa_ = GenerateArray(width, height, false);
        print("Array Generated");
        RenderMap(ground_base_fa_, 
            GeneralFunctions.GetTilemap(gameObject, "Tilemap - Ground - Base"),
            GeneralFunctions.GetTileBase("grass_block"));
        print("Map Rendered");
    }

    public int[,] GenerateArray(int width, int height, bool isEmpty)
    {
        int[,] tempArray = new int[width, height];
        int flag = isEmpty ? 0 : 1;
        for (int x = 0; x < tempArray.GetUpperBound(0); x++)
        {
            for (int y = 0; y < tempArray.GetLowerBound(1); y++)
            {
                tempArray[x, y] = flag;
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
        tilemap.ClearAllTiles();
        Vector3Int tile_position = new Vector3Int(0, 0, 0);
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (map[x,y] == 0)
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
