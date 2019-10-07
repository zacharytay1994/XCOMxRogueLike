using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityAttack
{
    [SerializeField] private int damage;
    [SerializeField] private int range;
    [SerializeField] private int aoe;

    public enum Attack
    {
        melee,
        range,
        aoe
    }

    public List<Vector3Int> ExecuteMelee()
    {
        return 0;
    }

    public List<Vector3Int> ExecuteRange()
    {
        return 0;
    }

    public List<Vector3Int> ExecuteAOE()
    {
        return 0;
    }
}
