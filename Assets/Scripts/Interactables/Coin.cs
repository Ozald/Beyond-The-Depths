using UnityEngine;

public class Coin : Interactable
{
    public override void Interact(PlayerInteraction player)
    {
        base.Interact(player);
        
        player.player.GetComponent<StatsManager>().doubloons += 1;
        Destroy(this.gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StatsManager statsManager = collision.GetComponent<StatsManager>();

            if (statsManager != null)
            {
                statsManager.doubloons += 1;
                Destroy(gameObject);
            }
        }
    }
}