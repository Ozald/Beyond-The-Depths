using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public List<Weapon> playerInv;
    private bool isInTriggerZone = false;
    private Weapon nearbyWeapon = null;
    private float lastAttackTime = 0f;


    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInTriggerZone && Input.GetKeyDown("e"))
        {
            //PickupWeapon();
        }

        if (Input.GetKeyDown("q"))
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

        if (Input.GetMouseButtonDown(0))
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
            /*
            Weapon currentWeapon = playerInv[0];
            currentWeapon.transform.SetParent(this.transform, false);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
            */

            Weapon currentWeapon = playerInv[0];

            Vector3 mousePos = Input.mousePosition;
            Vector3 viewportPos = new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0);

            float distanceToWeapon = transform.position.z - Camera.main.transform.position.z;
            mousePos = Camera.main.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, distanceToWeapon));

            Vector3 attackDir = mousePos - transform.position;
            float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

            currentWeapon.transform.position = transform.position + (attackDir).normalized - new Vector3(0, 0.6f, 0);
            currentWeapon.transform.rotation = Quaternion.Lerp(currentWeapon.transform.rotation, Quaternion.Euler(0, 0, angle - 90), 0.2f);
        }
    }

    public void PickupWeapon(Weapon nearbyWeapon)
    {
        if (nearbyWeapon == null) return;

        if (playerInv.Count < 1)
        {
            playerInv.Add(nearbyWeapon);
            //nearbyWeapon.gameObject.SetActive(false);
        }
        else if (playerInv.Count == 1)
        {
            playerInv.Add(nearbyWeapon);
            Weapon temp = playerInv[1];
            playerInv[1] = playerInv[0];
            playerInv[0] = temp;

            playerInv[0].gameObject.SetActive(true);
            playerInv[1].gameObject.SetActive(false);

        }
        else
        {
            Weapon droppedWeapon = playerInv[0];

            droppedWeapon.transform.position = nearbyWeapon.transform.position;
            droppedWeapon.transform.rotation = new Quaternion(0, 0, 0, 0);
            droppedWeapon.gameObject.SetActive(true);

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
