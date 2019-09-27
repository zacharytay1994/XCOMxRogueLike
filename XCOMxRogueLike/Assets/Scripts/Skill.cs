using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    private int skill_id_;
    private string skill_name_;
    private int skill_level_;
    private bool skill_unlocked_;

    //Function to level up existing skill
    public void SkillLevelUp(Skill SelectedSkill, Player player)
    {
        if (SelectedSkill.skill_unlocked_)
        {
            SelectedSkill.skill_level_ += 1;
            player.AddSkillLevel(1);
        }
    }

    //Function to unlock new skill
    public void UnlockNewSkill(Skill PrereqSkill, Skill NewSkill, Player player)
    {
        if(PrereqSkill.skill_unlocked_)
        {
            NewSkill.skill_unlocked_ = true;
            NewSkill.skill_level_ += 1;
            player.AddSkillLevel(1);
        }
    }
}
