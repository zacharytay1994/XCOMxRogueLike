using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLoader : MonoBehaviour
{
    private SceneTransitionLoader managerobject;
    

    // Start is called before the first frame update
    void Start()
    {
        managerobject = FindObjectOfType<SceneTransitionLoader>();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void load_scene_Asynch(string sceneName)
    {
        if (managerobject != null)
        {
            managerobject.load_scene_Asynch(sceneName);
        }
        else
        {
            Debug.LogError("Scenemanager not found. Start game from SplashScene to generate Scenemanager.");
        }
    }

    public void quit_game()
    {
        Application.Quit();
    }
   
}
