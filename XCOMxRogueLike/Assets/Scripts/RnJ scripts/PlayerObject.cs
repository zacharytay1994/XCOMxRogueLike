using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    [SerializeField] private int player_turns_;
    [SerializeField] private int player_action_points_;
    
    [SerializeField] private List<SkillObject> player_skills_;
    [SerializeField] private Weapon player_equipped_weapon_;
}
