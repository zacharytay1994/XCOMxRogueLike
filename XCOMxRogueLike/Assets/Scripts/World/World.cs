using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class World : MonoBehaviour
{
    public int number_of_creepers_ = 10;
    public Pathfinding pathfinding_ = new Pathfinding();
    public CustTilemap tilemap_ = new CustTilemap();

    // Start is called before the first frame update
    void Start()
    {
        tilemap_.WorldInit(pathfinding_);
        LoadEntities();
    }

    // Loads entity prefabs from Assets/Prefabs/Entities/Creeper.prefab
    void LoadEntities()
    {
        // get creeper prefab
        GameObject creeper_prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Entities/Creeper.prefab", typeof(GameObject));
        Transform parent_transform = GameObject.Find("Entities").transform;
        for (int i = 0; i < 10; i++)
        {
            Instantiate(creeper_prefab, parent_transform).GetComponent<EntityMove>().WorldReadyInit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
