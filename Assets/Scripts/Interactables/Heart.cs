public class Heart : Interactable
{
    public override void Interact(PlayerInteraction player)
    {
        int health = player.player.GetComponent<StatsManager>().currentHP;
        int maxHealth = player.player.GetComponent<StatsManager>().maxHealth.value;

        if (health < maxHealth)
        {
            player.player.GetComponent<StatsManager>().currentHP += 1;
            Destroy(this.gameObject);
        }
    }
}