using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity { }
[System.Serializable]
public class EntityAttack
{
    [SerializeField] private int damage_;
    [SerializeField] private int range_;
    [SerializeField] private int aoe_;

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

    // Targeted Ranged Attacks
    public bool CheckWithinRange(Constants.RoomTile[,] tile_list, List<Vector3Int> aoe_coordinates_list, ref List<Entity> entitylist)
    {
        bool flag = false;
        foreach(Vector3Int coordinate in aoe_coordinates_list)
        {
            if (tile_list[coordinate.x, coordinate.y].is_occupied_)
            {
                //if(tile_list[coordinate.x, coordinate.y].entity_.is_enemy_)
                //{
                //      entity_list.Add(tile_list[coordinate.x, coordinate.y].entity_);  
                //}
                flag = true;
            }
        }
        return flag;
    }
}
