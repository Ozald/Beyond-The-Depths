using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WeightedItem
{
    public Interactable item;
    public int weight;
}

public class Chest : Interactable
{
    public WeightedItem[] items;
    private ItemSelector<Interactable> LootPool;
    public bool isOpen = false;
    public Animator animator;

    void Start()
    {
        LootPool = new ItemSelector<Interactable>();

        foreach (WeightedItem item in items)
            LootPool.AddItem(item.item, item.weight);
    }

    public override void Interact(PlayerInteraction player)
    {
        if (!isOpen)
        {
            isOpen = true;

            this.enabled = false;

            animator.SetBool("IsOpen", true);

            StartCoroutine(OpenChestRoutine());
        }
    }

    private IEnumerator OpenChestRoutine()
    {
        yield return new WaitForSeconds(0.40f);

        Interactable item = LootPool.Roll();

        Instantiate(item, transform.position, transform.rotation);
        Destroy(this);
    }
}