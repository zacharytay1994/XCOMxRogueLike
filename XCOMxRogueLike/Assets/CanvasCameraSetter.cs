using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    [SerializeField] Camera current_camera;
    Canvas current_canvas;
    // Start is called before the first frame update
    void Start()
    {
        current_canvas = GetComponent<Canvas>();
        current_camera = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        if (!current_camera)
        {
            if (Camera.main != null)
            {
                current_camera = Camera.main;
                current_canvas.worldCamera = current_camera;
                Debug.Log("Loading Scene Camera Set!");
            }
            else
            {
                Debug.Log("No camera found!");
            }
        }
    }
}
