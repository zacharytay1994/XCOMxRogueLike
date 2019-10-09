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
    
    public List<Vector3Int> wide_range (Vector3Int wide_range_coordinate, int range)
    {
        List<Vector3Int> wide_range_list = new List<Vector3Int>();

        for (int y = 0; y <= range; y++)
        {
            int offset = range - y;

            for (int x = -offset; x <= offset; x++)
            {
                if (y != 0)
                {
                    wide_range_list.Add(new Vector3Int(wide_range_coordinate.x + x, wide_range_coordinate.y + y, 0));
                    wide_range_list.Add(new Vector3Int(wide_range_coordinate.x + x, wide_range_coordinate.y - y, 0));
                }
                else
                {
                    wide_range_list.Add(new Vector3Int(wide_range_coordinate.x + x, wide_range_coordinate.y, 0));
                }
            }
        }

        return wide_range_list;
    }

    // Returns list of tiles' coordinates within melee range
    public List<Vector3Int> MeleeRange(Vector3Int player_position_, bool attack_type_)
    {
        List<Vector3Int> melee_range_list = new List<Vector3Int>();
        Vector3Int tile_pos_to_add;

        // Swinging Attack Type Range
        if (attack_type_ == false)
        {
            for (int x = player_position_.x - 1; x <= player_position_.x + 1; x++)
            {
                for (int y = player_position_.y - 1; y <= player_position_.y + 1; y++)
                {
                    tile_pos_to_add = new Vector3Int(x, y, 0);

                    if (tile_pos_to_add != player_position_)
                    {
                        melee_range_list.Add(tile_pos_to_add);
                    }
                }
            }
        }

        // Thrusting Attack Type Range
        else
        {
            for (int x = player_position_.x - 2; x <= player_position_.x + 2; x++)
            {
                tile_pos_to_add = new Vector3Int(x, player_position_.y, 0);

                if (tile_pos_to_add != player_position_)
                {
                    melee_range_list.Add(tile_pos_to_add);
                }
            }

            for (int y = player_position_.y - 2; y <= player_position_.y + 2; y++)
            {
                tile_pos_to_add = new Vector3Int(player_position_.x, y, 0);

                if (tile_pos_to_add != player_position_)
                {
                    melee_range_list.Add(tile_pos_to_add);
                }
            }
        }

        return melee_range_list;
    }

    // Returns list of enemies' coordinates in attack range
    public List<Entity> EnemiesWithinRange(Constants.RoomTile[,] tile_list, List<Vector3Int> attack_range_list)
    {
        List<Entity> enemy_in_range_list = new List<Entity>();

        foreach (Vector3Int coordinate in attack_range_list)
        {
            if (tile_list[coordinate.x, coordinate.y].is_occupied_)
            {
                /*if(tile_list[coordinate.x, coordinate.y].entity_.is_enemy_)
                {
                      enemy_in_range_list.Add(tile_list[coordinate.x, coordinate.y].entity_);  
                }*/
            }
        }

        return enemy_in_range_list;
    }

    #region CheckForEnemyMelee function currently not in use
    //checks for enemy on tiles that are in range for melee attacks
    public bool CheckForEnemyMelee(Constants.RoomTile[,] tile_list)
    {
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
    #endregion
}
