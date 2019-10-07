using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    [SerializeField] string bow_name_;
    [Space(10)]
    [SerializeField] int bow_base_attack_;
    [SerializeField] int bow_base_range_;
    [SerializeField] int bow_base_aoe_;
    [SerializeField] float bow_minimum_power_;
    [SerializeField] float bow_maximum_power_;


    public override void GenerateNewRandomStats()
    {
        float generate_power = Random.Range(bow_minimum_power_, bow_maximum_power_);
        weapon_attack_ = Mathf.RoundToInt(generate_power * bow_base_attack_);
        weapon_range_ = Mathf.RoundToInt(generate_power * bow_base_range_);
        weapon_aoe_ = Mathf.RoundToInt(generate_power * bow_base_aoe_);
        weapon_name_ = DecideWeaponTitle(bow_minimum_power_, bow_maximum_power_, generate_power) + " " + bow_name_;
    }
}
