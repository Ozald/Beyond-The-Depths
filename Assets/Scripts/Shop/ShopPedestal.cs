using TMPro;
using UnityEngine;

public class ShopPedestal : Interactable
{
    private float counter = 0;
    public ShopItems ItemPool;
    public bool purchased = false;
    public TextMeshPro costTest;
    private SellableItem sellableItem;
    private Interactable item;

    void Start()
    {
        sellableItem = ItemPool.GetItem();
        item = sellableItem.item;
        
        if (item is not null) 
        {
            Instantiate(item, gameObject.transform.position + Vector3.up, Quaternion.identity);
            item.CanInteract = false;
            costTest.text = sellableItem.price.ToString();
        }
    }

    public void Purchase(PlayerInteraction player)
    {
        if (item is null)
            return;
        
        if (player.player.GetComponent<StatsManager>().doubloons >= sellableItem.price)
        {
            if (purchased)
                return;

            purchased = true;
            CanInteract = false;
            item.CanInteract = true;
            player.player.GetComponent<StatsManager>().doubloons -= sellableItem.price;
            gameObject.GetComponent<ParticleSystem>().Stop();
        }
    }

    public override void Interact(PlayerInteraction player)
    {
        Purchase(player);
    }
}
