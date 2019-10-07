using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputs : MonoBehaviour
{
    [SerializeField] GameObject skillTree;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            if(skillTree.activeInHierarchy)
            {
                skillTree.SetActive(false);
            }
            else
            {
                skillTree.SetActive(true);
            }
        }
    }
}
