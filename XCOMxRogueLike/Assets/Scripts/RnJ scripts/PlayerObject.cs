using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private int player_turns_;
    [SerializeField] private int player_action_points_;
    
    [SerializeField] private List<SkillObject> player_skills_;
    [SerializeField] private Weapon player_equipped_weapon_;

    void Update()
    {
        
    }

    public override int CalculateMaxHealth()
    {
        //base health + skill health + weapon health
        return entity_max_health_;
    }


    #region player attacking
    // PLAYER ATTACKING ENEMY FUNCTIONS--------------------
    public void CalculateRange(Enemy enemy_position)
    {
        //if (enemy_position_ < player_range_)
        //{
        //    CalculateAOE();
        //}
        //else
        //{
        //    //Inform player that enemy is out of range
        //}
    }

    public void CalculateAOE()
    {
        //Check all surrounding entities within attack's AOE
    }

    public int CalculateDamage()
    {
        //player_attack_ = weapon_attack_ + skill_damage_;

        return entity_attack_;
    }

    public int DamageDoneToEnemy()
    {
        int Damage_Done = 0;

        //Damage_Done = CalculateDamage() * weapon_onhit_amount_;

        return Damage_Done;
    }
    // END OF PLAYER ATTACKING ENEMIES FUNCTION-------------
    #endregion

    public override void TakeDamage(int DamageTaken)
    {
        entity_current_health_ -= DamageTaken;
    }

    public void CalculateTurnsTaken()
    {
        player_turns_ += 1;
    }
}
