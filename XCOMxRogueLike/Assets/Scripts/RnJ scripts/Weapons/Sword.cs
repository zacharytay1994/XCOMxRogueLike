using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    [SerializeField] string sword_name_;
    [Space(10)]
    [SerializeField] int sword_base_attack_;
    [SerializeField] int sword_base_range_;
    [SerializeField] int sword_base_aoe_;
    [SerializeField] float sword_minimum_power_;
    [SerializeField] float sword_maximum_power_;
    

    public override void GenerateNewRandomStats()
    {
        float generate_power = Random.Range(sword_minimum_power_, sword_maximum_power_);
        weapon_attack_ = Mathf.RoundToInt(generate_power * sword_base_attack_);
        weapon_range_ = Mathf.RoundToInt(generate_power * sword_base_range_);
        weapon_aoe_ = Mathf.RoundToInt(generate_power * sword_base_aoe_);
        weapon_name_ = DecideWeaponTitle(sword_minimum_power_, sword_maximum_power_, generate_power) + " " + sword_name_;
    }
}
