using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Weapon> playerInv;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Things happened");
        Weapon weapon = collision.gameObject.GetComponent<Weapon>();
        if (collision.gameObject.CompareTag("Weapon"))
        {
            playerInv.Add(weapon);
            collision.gameObject.SetActive(false);
        }
    }
}
