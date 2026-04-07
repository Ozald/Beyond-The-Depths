using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public Image Weapon1;
    public Image Weapon2;
    public PlayerInventory playerInventory;

    void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Slot 1
        if (playerInventory.playerInv.Count > 0 && playerInventory.playerInv[0] != null)
        {
            Weapon1.sprite = playerInventory.playerInv[0].GetComponent<SpriteRenderer>().sprite;
            Weapon1.enabled = true;
        }
        else
        {
            Weapon1.enabled = false;
        }

        // Slot 2
        if (playerInventory.playerInv.Count > 1 && playerInventory.playerInv[1] != null)
        {
            Weapon2.sprite = playerInventory.playerInv[1].GetComponent<SpriteRenderer>().sprite;
            Weapon2.enabled = true;
        }
        else
        {
            Weapon2.enabled = false;
        }
    }
}