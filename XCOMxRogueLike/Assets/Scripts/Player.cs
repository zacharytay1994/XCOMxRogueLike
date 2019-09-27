using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int player_health_;
    private int player_attack_;
    private int player_range_;
    private int player_aoe_;
    private int player_experience_;
    private int player_level_;
    private int player_exp_requisite_;
    private int player_skills_;
    private int player_total_skill_level_;
    private int player_turns_;

    void Update()
    {
        CheckPlayerSkills();
    }

    public void AddSkillLevel(int incrementAmount)
    {
        player_total_skill_level_ += incrementAmount;
    }

    public void CheckPlayerSkills()
    {
        if (player_level_ > player_total_skill_level_)
        {
            //Allow player to put skill point in
        }
    }

    public void PlayerLevelUp()
    {
        int player_balance_experience = 0;

        //Increase Player level by 1
        player_level_ += 1;

        //When killing an enemy, if experience gained exceeds player_exp_requisite, spill over to next level
        if (player_experience_ > player_exp_requisite_)
        {
            player_balance_experience = player_experience_ - player_exp_requisite_;
        }

        //Increase player_exp_requisite for next level
        player_exp_requisite_ = player_exp_requisite_ * player_level_;

        //Reset player_experience and spill balance experience into next level
        player_experience_ = 0 + player_balance_experience;
    }

    public void CalculateRange(Enemy enemy_position)
    {
        if(enemy_position_ < player_range_)
        {
            CalculateAOE();
        }
        else
        {
            //Inform player that enemy is out of range
        }
    }

    public void CalculateAOE()
    {
        //Check all surrounding entities within attack's AOE
    }

    public int CalculateDamage()
    {
        player_attack_ = weapon_attack_ + skill_damage_;

        return player_attack_;
    }

    public int DamageDoneToEnemy()
    {
        int Damage_Done = 0;

        Damage_Done = CalculateDamage() * weapon_onhit_amount_;

        return Damage_Done;
    }

    public void PlayerTakeDamage(int DamageTaken)
    {
        player_health_ -= DamageTaken;
    }

    public void CalculateTurnsTaken()
    {
        //For every turn taken, increment player_turns_
    }
}
