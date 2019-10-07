using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class GeneralFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Tilemap GetTilemap(GameObject parentobject, string tilemapname)
    {
        Transform child_transform = parentobject.transform.Find(tilemapname);
        if (child_transform != null)
        {
            return (Tilemap)child_transform.gameObject.GetComponent(typeof(Tilemap));
        }
        else
        {
            return null;
        }
    }

    // Gets a TileBase object from the asset folder with the asset name, e.g. GetTileBase("grass_block")
    public static TileBase GetTileBase(string tilename)
    {
        return (TileBase)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Tiles/" + tilename + ".asset", typeof(TileBase));
    }

    public static string GetTileSpriteName(Tilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile<Tile>(position).sprite.name;
    }
}
