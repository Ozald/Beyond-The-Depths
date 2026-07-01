public class Heart : Interactable
{
    public override void Interact(PlayerInteraction player)
    {
        float health = player.player.GetComponent<StatsManager>().currentHP;
        float maxHealth = player.player.GetComponent<StatsManager>().maxHealth.value;

        if (health < maxHealth)
        {
            player.player.GetComponent<StatsManager>().currentHP += 1;
            Destroy(this.gameObject);
        }
    }
}