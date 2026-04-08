using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Interactable
{
    public WeaponData weaponData;
    public override void Interact(PlayerInteraction player)
    {
        PlayerInventory inv = player.GetComponent<PlayerInventory>();

        if (inv != null && !inv.playerInv.Contains(this))
        {
            inv.PickupWeapon(this);
        }
        else
        {
            Debug.LogWarning("PlayerInventory not found on player!");
        }
    }
}
