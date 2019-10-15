using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    [SerializeField] private List<SkillObject> all_skills_list_
    // Start is called before the first frame update
    void Start()
    {
        InitSkills();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitSkills()
    {
        foreach(SkillObject s in all_skills_list_)
        {
            if(s.Skill_level_>0)
            {
                s.UnlockSkillsInPrerequisite();
            }
        }
    }
}
