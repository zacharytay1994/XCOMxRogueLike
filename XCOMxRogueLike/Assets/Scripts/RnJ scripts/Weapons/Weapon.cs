using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : MonoBehaviour
{
    public enum type
    {
        sword,
        bow,
        hammer
    }
    [SerializeField] protected List<string> weapon_title_;
    [SerializeField] protected string weapon_name_;
    [SerializeField] protected int weapon_attack_;
    [SerializeField] protected int weapon_range_;
    [SerializeField] protected int weapon_aoe_;

    public string Weapon_name_ { get => weapon_name_; set => weapon_name_ = value; }
    public int Weapon_attack_ { get => weapon_attack_; set => weapon_attack_ = value; }
    public int Weapon_range_ { get => weapon_range_; set => weapon_range_ = value; }
    protected int Weapon_aoe_ { get => weapon_aoe_; set => weapon_aoe_ = value; }

    public virtual void GenerateNewRandomStats() { }

    public string DecideWeaponTitle(float min_power, float max_power, float generated_power)
    {
        float power_range = max_power - min_power;
        float power_divider;
        for(int i = 0;i<weapon_title_.Count;i++)
        {
            power_divider = ((i + 1.0f) / weapon_title_.Count * power_range) + min_power; //CAN SOMEONE TELL ME WHY THIS CODE ISNT WORKING
            Debug.Log(power_divider);
            if(power_divider>=generated_power)
            {
                return weapon_title_[i];
            }
        }
        return null;
    }
}

