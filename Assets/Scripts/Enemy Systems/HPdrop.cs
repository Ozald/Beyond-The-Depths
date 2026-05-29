using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPdrop : MonoBehaviour
{
    int amount = 1;
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
        if (collision.CompareTag("Player"))
        {
            StatsManager statsManager = collision.GetComponent<StatsManager>();

            if (statsManager != null && statsManager.currentHP < statsManager.maxHealth.value)
            {
                statsManager.currentHP++;

                Destroy(gameObject);

                Debug.Log("Player healed by " + amount + " HP | Current HP: " + statsManager.currentHP);
            }
            else
            {
                Debug.Log("Player did not healed | HP is full.");
            }
        }
    }
}
