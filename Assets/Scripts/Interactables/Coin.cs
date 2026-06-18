public class Coin : Interactable
{
    public override void Interact(PlayerInteraction player)
    {
        player.player.GetComponent<StatsManager>().doubloons += 1;
        Destroy(this.gameObject);
    }
}