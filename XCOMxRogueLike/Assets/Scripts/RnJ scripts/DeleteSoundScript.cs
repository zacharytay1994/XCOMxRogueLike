using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSoundScript : MonoBehaviour
{
    AudioSource aus;
    // Start is called before the first frame update
    void Start()
    {
        aus = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!aus.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}


