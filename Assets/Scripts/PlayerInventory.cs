using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Weapon> playerInv;
    private bool isInTriggerZone = false;
    private Weapon nearbyWeapon = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isInTriggerZone && Input.GetKeyDown("e"))
        {
            PickupWeapon();
        }

        if (Input.GetKeyDown("r"))
        {
            Weapon temp = playerInv[1];
            playerInv[1] = playerInv[0];
            playerInv[0] = temp;
        }
    }

    private void PickupWeapon()
    {
        if (nearbyWeapon == null) return;

        if (playerInv.Count <= 1)
        {
            playerInv.Add(nearbyWeapon);
            nearbyWeapon.gameObject.SetActive(false);
        }
        else
        {
            Weapon droppedWeapon = playerInv[1];

            droppedWeapon.transform.position = nearbyWeapon.transform.position;
            droppedWeapon.gameObject.SetActive(true);

            playerInv[1] = playerInv[0];
            playerInv[0] = nearbyWeapon;

            nearbyWeapon.gameObject.SetActive(false);
        }

        isInTriggerZone = false;
        nearbyWeapon = null;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            isInTriggerZone = true;
            nearbyWeapon = collision.gameObject.GetComponent<Weapon>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            isInTriggerZone = false;
            nearbyWeapon = null;
        }
    }
}
