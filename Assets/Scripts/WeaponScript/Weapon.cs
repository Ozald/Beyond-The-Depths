using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Interactable
{
    public WeaponData weaponData;
    public override void Interact(PlayerInteraction player)
    {
        PlayerInventory inv = player.GetComponent<PlayerInventory>();

        if (inv != null)
        {
            inv.PickupWeapon(this);
        }
        else
        {
            Debug.LogWarning("PlayerInventory not found on player!");
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
