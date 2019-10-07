using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats
{
#region Entity Health Stats
    [SerializeField] protected int entity_max_health_;
    [SerializeField] protected int entity_current_health_;
    #endregion

#region Entity Movement Stats
    [SerializeField] protected int entity_action_point_;
    [SerializeField] protected int entity_movement_cost_;
    #endregion
}