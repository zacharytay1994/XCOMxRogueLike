using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionLoader : MonoBehaviour
{
    public static GameObject instance;

    public float load_progress;

    public bool scene_loading;
    public bool scene_loaded;

    public Animator Animator;

    private void Awake()    //Make this object persistent
    {
        if (!instance)
        {
            instance = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void load_scene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void load_scene_Asynch(string SceneName)
    {
        if (!scene_loading)
        {
            StartCoroutine(LoadSceneAsync(SceneName));
        }
    }

    IEnumerator LoadSceneAsync(string SceneName)
    {
        scene_loading = true;
        scene_loaded = false;
        FadeToBlack();

        yield return new WaitForSeconds(0.3f);

        AsyncOperation async = SceneManager.LoadSceneAsync(SceneName);
        while (!async.isDone)
        {
            load_progress = Mathf.Clamp01(async.progress / 0.9f);
            Debug.Log("Load progress: " + load_progress * 100 + "%");
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        scene_loading = false;
        scene_loaded = true;
        FadeFromBlack();

    }

    public void FadeToBlack()
    {
        Animator.SetBool("scene_loading", true);
        Animator.SetBool("scene_loaded", false);

    }

    public void FadeFromBlack()
    {
        Animator.SetBool("scene_loading", false);
        Animator.SetBool("scene_loaded", true);
    }
}
