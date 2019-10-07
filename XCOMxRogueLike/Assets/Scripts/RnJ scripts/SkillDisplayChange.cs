using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDisplayChange : MonoBehaviour
{
    [SerializeField] Image skill_image_;
    [SerializeField] TextMeshProUGUI skill_name_;
    [SerializeField] TextMeshProUGUI skill_level_;
    [SerializeField] TextMeshProUGUI skill_cost_;
    [SerializeField] TextMeshProUGUI skill_description_;
    SkillObject current_selected_skill_;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSkillDisplay(SkillObject skill)
    {
        current_selected_skill_ = skill;
        skill_image_.sprite = skill.Skill_icon_;
        skill_name_.text = skill.Skill_name_;
        skill_level_.text = "Level: " + skill.Skill_level_;
        skill_cost_.text = "Cost: " + skill.Skill_current_cost_;
        skill_description_.text = skill.Skill_description_;
    }

    public void SkillLevelUp()
    {
        current_selected_skill_.SkillLevelUp();
        ChangeSkillDisplay(current_selected_skill_);
    }
}
