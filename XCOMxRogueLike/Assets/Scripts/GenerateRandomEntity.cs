using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRandomEntity : MonoBehaviour
{
    public GameObject[] game_objects_;
    // Start is called before the first frame update
    void Start()
    {
        int random_number = Random.Range(0, game_objects_.Length);
        Instantiate(game_objects_[random_number], transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
