public class Upgrade : Interactable
{
    public UpgradeStats stats;

    public override void Interact(PlayerInteraction player)
    {
        UpgradeInventory inv = player.GetComponent<UpgradeInventory>();
        StatsManager manager = player.GetComponent<StatsManager>();
        
        if (inv is not null && manager is not null)
        {
            inv.PickupUpgrade(this);
            stats.Apply(manager);
        }
        
        Destroy(gameObject);
    }
}
