using System.Collections;
using UnityEngine;

public class Pot : Interactable
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
        
        Debug.Log("Item array size: " + items.Length);
        Debug.Log("Loot pool items: " + LootPool.Size);
    }

    public override void Interact(PlayerInteraction player)
    {
        if (!isOpen)
        {
            isOpen = true;

            this.enabled = false;

            //animator.SetBool("IsOpen", true);

            StartCoroutine(OpenPot());
        }
    }

    private IEnumerator OpenPot()
    {
        yield return new WaitForSeconds(0.2f);

        Interactable item = LootPool.Roll();

        Instantiate(item, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
        
