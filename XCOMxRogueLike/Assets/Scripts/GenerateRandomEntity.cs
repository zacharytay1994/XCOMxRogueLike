using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateRandomEntity : MonoBehaviour
{
    public GameObject[] game_objects_;
    // Start is called before the first frame update
    void Start()
    {
        Tilemap tilemap = (Tilemap)Object.FindObjectOfType(typeof(Tilemap));
        Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
       
       // Debug.Log(cellPosition + "Hi");
        int random_number = Random.Range(0, game_objects_.Length);
       // Debug.Log(tilemap.GetCellCenterWorld(cellPosition));
        Instantiate(game_objects_[random_number], tilemap.GetCellCenterWorld(cellPosition) , Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
