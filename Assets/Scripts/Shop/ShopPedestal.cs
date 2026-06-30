using TMPro;
using UnityEngine;

public class ShopPedestal : Interactable
{
    public Interactable item;
    public int cost;
    private float counter = 0;
    public bool purchased = false;
    public TextMeshPro costTest;

    void Start()
    {
        if (item is not null) 
        {
            Instantiate(item, gameObject.transform.position + Vector3.up, Quaternion.identity);
            item.CanInteract = false;
            costTest.text = cost.ToString();
        }
    }

    public void Purchase(PlayerInteraction player)
    {
        if (item is null)
            return;
        
        if (player.player.GetComponent<StatsManager>().doubloons >= cost)
        {
            if (purchased)
                return;

            purchased = true;
            CanInteract = false;
            item.CanInteract = true;
            player.player.GetComponent<StatsManager>().doubloons -= cost;
            gameObject.GetComponent<ParticleSystem>().Stop();
        }
    }

    public override void Interact(PlayerInteraction player)
    {
        Purchase(player);
    }
}
