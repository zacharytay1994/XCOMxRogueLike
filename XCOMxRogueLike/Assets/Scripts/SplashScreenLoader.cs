using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenLoader : MonoBehaviour
{
    [SerializeField] string start_scene_name;
    private SceneTransitionLoader s_l;
    [SerializeField] bool load_scene;
    // Start is called before the first frame update
    void Start()
    {
        s_l = FindObjectOfType<SceneTransitionLoader>();

        if (load_scene)
        {
            if (start_scene_name != null)
            {
                s_l.load_scene_Asynch(start_scene_name);
            }
            else
            {
                Debug.Log("Bruh enter the menu scene name properly");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (load_scene)
        {
            Load_scene();
        }
    }

    public void Load_scene()
    {
        if (start_scene_name != null)
        {
            s_l.load_scene_Asynch(start_scene_name);
        }
        else
        {
            Debug.Log("Bruh enter the menu scene name properly");
        }
    }
}
