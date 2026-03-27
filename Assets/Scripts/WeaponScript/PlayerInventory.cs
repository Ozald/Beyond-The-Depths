using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Weapon> playerInv;
    private bool isInTriggerZone = false;
    private Weapon nearbyWeapon = null;
    private float lastAttackTime = 0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isInTriggerZone && Input.GetKeyDown("e"))
        {
            //PickupWeapon();
        }

        if (Input.GetKeyDown("r"))
        {
            if (playerInv.Count > 1)
            {
                Weapon temp = playerInv[1];
                playerInv[1] = playerInv[0];
                playerInv[0] = temp;

                playerInv[0].gameObject.SetActive(true);
                playerInv[1].gameObject.SetActive(false);
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed");
            if (playerInv.Count == 0) return;

            Weapon currentWeapon = playerInv[0];

            if (currentWeapon == null || currentWeapon.weaponData == null) return;

            float attackSpeed = currentWeapon.weaponData.cooldown;

            if (Time.time - lastAttackTime < attackSpeed) return;

            lastAttackTime = Time.time;

            currentWeapon.weaponData.Attack(gameObject);
            Debug.Log("Attack done");
        }

        if (playerInv.Count > 0 && playerInv[0] != null)
        {
            Weapon currentWeapon = playerInv[0];
            currentWeapon.transform.SetParent(this.transform, false);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
    }

    public void PickupWeapon(Weapon nearbyWeapon)
    {
        if (nearbyWeapon == null) return;

        if (playerInv.Count <= 1)
        {
            playerInv.Add(nearbyWeapon);
            //nearbyWeapon.gameObject.SetActive(false);
        }
        else
        {
            Weapon droppedWeapon = playerInv[1];

            droppedWeapon.transform.position = nearbyWeapon.transform.position;
            droppedWeapon.gameObject.SetActive(true);

            playerInv[1] = playerInv[0];
            playerInv[0] = nearbyWeapon;

            //nearbyWeapon.gameObject.SetActive(false);
            playerInv[1].gameObject.SetActive(false);
        }

        isInTriggerZone = false;
        nearbyWeapon = null;


    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Weapon weapon = collision.gameObject.GetComponent<Weapon>();
        if (weapon != null)
        {
            isInTriggerZone = true;
            nearbyWeapon = collision.gameObject.GetComponent<Weapon>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Weapon weapon = collision.gameObject.GetComponent<Weapon>();
        if (weapon != null)
        {
            isInTriggerZone = false;
            nearbyWeapon = null;
        }
    }
}
