using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWeaponGeneratorScript : MonoBehaviour
{
    [SerializeField] List<GameObject> weapon_prefabs_;
    [SerializeField] private List<Weapon> weapon_list_;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateNewWeapon()
    {
        GameObject newWeapon = Instantiate(weapon_prefabs_[Random.Range(0, weapon_prefabs_.Count)]);
        Weapon w = newWeapon.GetComponent<Weapon>();
        w.GenerateNewRandomStats();
        weapon_list_.Add(w);
    }
}
