public class Coin : Interactable
{
    public override void Interact(PlayerInteraction player)
    {
        base.Interact(player);
        
        player.player.GetComponent<StatsManager>().doubloons += 1;
        Destroy(this.gameObject);
    }
}