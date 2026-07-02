using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WeightedItem
{
    public GameObject item;
    public bool LuckAffected;
    public int weight;
}

public class Chest : Interactable
{
    public WeightedItem[] items;
    private ItemSelector<GameObject> LootPool;
    public bool isOpen = false;
    public Animator animator;

    void Start()
    {
        LootPool = new ItemSelector<GameObject>();
        
        Debug.Log("Item array size: " + items.Length);
        Debug.Log("Loot pool items: " + LootPool.Size);
    }

    public override void Interact(PlayerInteraction player)
    {
        if (!isOpen)
        {
            foreach (WeightedItem item in items)
            {
                if(item.LuckAffected) 
                    LootPool.AddItem(item.item, Math.Max( // Math.Max is there to just prevent issues with negative luck
                        1, item.weight + player.player.GetComponent<StatsManager>().luck.value));
                else
                    LootPool.AddItem(item.item, item.weight);
            }

            isOpen = true;

            this.enabled = false;

            animator.SetBool("IsOpen", true);

            StartCoroutine(OpenChestRoutine());
        }
    }

    private IEnumerator OpenChestRoutine()
    {
        yield return new WaitForSeconds(0.40f);

        GameObject item = LootPool.Roll();

        Instantiate(item, transform.position, transform.rotation);
        Destroy(this);
    }
}