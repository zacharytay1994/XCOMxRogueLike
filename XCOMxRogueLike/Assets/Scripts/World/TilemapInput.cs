using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInput : MonoBehaviour
{
    private Tilemap tilemap_;

    // Start is called before the first frame update
    void Start()
    {
        tilemap_ = GameObject.Find("BaseLayer").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = tilemap_.WorldToCell(mousePos);

            if (tilemap_.HasTile(gridPos))
                Debug.Log("Hello World from " + (gridPos));   
        }
    }
}
