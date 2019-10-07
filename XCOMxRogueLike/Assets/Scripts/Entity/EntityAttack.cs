using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity { }
[System.Serializable]
public class EntityAttack : MonoBehaviour
{

    private Vector3Int selected_tile_;

    [SerializeField] private int damage_;
    [SerializeField] private int range_;
    [SerializeField] private int aoe_;


    public enum Attack
    {
        melee,
        range,
        aoe
    }



    /*public List<Vector3Int> ExecuteRange()
    {
        return 0;
    }*/

    public List<Vector3Int> ExecuteAOE(Vector3Int aoe_coordinate, int aoe)
    {
        List<Vector3Int> aoe_coordinate_list = new List<Vector3Int>();

        for (int y = 0; y <= aoe; y++)
        {
            int offset = aoe - y;

            for (int x = -offset; x <= offset; x++)
            {
                if (y != 0)
                {
                    aoe_coordinate_list.Add(new Vector3Int(aoe_coordinate.x + x, aoe_coordinate.y + y, 0));
                    aoe_coordinate_list.Add(new Vector3Int(aoe_coordinate.x + x, aoe_coordinate.y - y, 0));
                }
                else
                {
                    aoe_coordinate_list.Add(new Vector3Int(aoe_coordinate.x + x, aoe_coordinate.y, 0));
                }
            }
        }

        return aoe_coordinate_list;
    }

    public bool CheckForEnemyMelee(Constants.RoomTile[,] tile_list) { //checks for enemy on tiles that are in range for melee attacks
        bool flag = false;
            // get mouse click's position in 2d plane
        Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pz.z = 0;

        // convert mouse click's position to Grid position
        GridLayout gridLayout = transform.parent.GetComponentInParent<GridLayout>();
        Vector3Int cellPosition = gridLayout.WorldToCell(pz);

       // if(CheckMeleeRange(gameObject.transform ,cellPosition, attack_type_))

        if (tile_list[cellPosition.x, cellPosition.y].is_occupied_)
        {
          /*  if(tile_list[cellPosition.x, cellPosition.y].is_enemy_)
            {
                flag = true;
            }*/
        }
        return flag;
    }

    public bool CheckMeleeRange(Vector3Int player_position_, Vector3Int selected_tile_, bool attack_type_) //checks what tiles are in range for melee attacks
    {
        bool inrange = false;
        if (attack_type_ == false)
        {
            if (selected_tile_.x >= player_position_.x - 1 && selected_tile_.x <= player_position_.x + 1)
            {
                if (selected_tile_.y >= player_position_.y - 1 && selected_tile_.y <= player_position_.y + 1)
                {
                    inrange = true;
                }

            }
        }
        else
        {
            if (selected_tile_.x >= player_position_.x - 2 && selected_tile_.x <= player_position_.x + 2 && selected_tile_.y == player_position_.y)
            {
                inrange = true;
            }
            else if (selected_tile_.y >= player_position_.y - 2 && selected_tile_.y <= player_position_.y + 2 && selected_tile_.x == player_position_.x)
            {
                inrange = true;
            }
            else
            {
                inrange = false;
            }

        }

        return inrange;
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
