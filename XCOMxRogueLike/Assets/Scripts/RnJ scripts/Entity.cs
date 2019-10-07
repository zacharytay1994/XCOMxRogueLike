using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected int entity_max_health_;
    [SerializeField] protected int entity_current_health_;
    [SerializeField] protected int entity_speed_;
    [SerializeField] protected int entity_range_;
    [SerializeField] protected int entity_aoe_;
    [SerializeField] protected int entity_attack_;

    
    public virtual int CalculateAttackValue()
    {
        return 0;
    }

    public virtual int CalculateRangeValue()
    {
        return 0;
    }

    public virtual int CalculateAOEValue()
    {
        return 0;
    }

    public virtual int CalculateSpeedValue()
    {
        return 0;
    }

    public virtual int CalculateMaxHealth()
    {
        return 0;
    }

    public virtual int CalculateCurrentHealth()
    {
        return 0;
    }

    public virtual void Move()
    {

    }

    public virtual void Attack()
    {

    }

    public virtual void CheckWithinRange(Entity target)
    {

    }

    public virtual void CheckWithinAOE()
    {

    }

    public virtual void TakeDamage(int DamageTaken)
    {

    }
}
