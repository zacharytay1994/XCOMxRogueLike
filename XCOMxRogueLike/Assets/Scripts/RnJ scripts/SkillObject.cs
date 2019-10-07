using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkillObject : MonoBehaviour
{
    [SerializeField] private string skill_name_;
    [SerializeField] private string skill_description_;

    //
    [SerializeField] private int skill_level_;
    [SerializeField] private int skill_base_cost_;
    [SerializeField] private int skill_current_cost_;
    [SerializeField] private bool skill_unlocked_;
    //
    [SerializeField] private Sprite skill_icon_;
    [SerializeField] List<SkillObject> skill_prerequisite_for_;
    Image skill_icon_image_;

    public Sprite Skill_icon_ { get => skill_icon_; set => skill_icon_ = value; }
    public string Skill_name_ { get => skill_name_; set => skill_name_ = value; }
    public string Skill_description_ { get => skill_description_; set => skill_description_ = value; }
    public int Skill_level_ { get => skill_level_; set => skill_level_ = value; }
    public int Skill_current_cost_ { get => skill_current_cost_; set => skill_current_cost_ = value; }
    public bool Skill_unlocked_ { get => skill_unlocked_; set => skill_unlocked_ = value; }

    private void Start()
    {
        skill_icon_image_ = GetComponent<Image>();
        skill_icon_image_.sprite = skill_icon_;
        skill_current_cost_ = CalculateSkillCost();
    }
    // Function to calculate the new cost of skill after levelling up
    public int CalculateSkillCost()
    {
        if(Skill_level_<1)
        {
            return skill_base_cost_;
        }
        else return (Skill_current_cost_ + skill_base_cost_) * Skill_level_;
    }

    //Function to level up existing skill
    public void SkillLevelUp(/*Player player*/)
    {
        if (Skill_unlocked_)
        {
            Skill_level_ += 1;
            if(Skill_level_==1)
            {
                UnlockSkillsInPrerequisite();
            }
            //player.AddSkillLevel(1);
            Skill_current_cost_ = CalculateSkillCost();
        }

    }

    public void UnlockSkillsInPrerequisite()
    {
        foreach(SkillObject s in skill_prerequisite_for_)
        {
            s.Skill_unlocked_ = true;
        }
    }
}
