/// <summary>
/// A shop to buy items or upgrades
/// </summary>
public class Shop : Interactable
{
    public ShopInventory inventory;
    // An inventory UI scene or something will be needed here
    // public ShopUI ShopInterface;

    public override void Interact(PlayerInteraction player)
    {
        // The ShopUI will need to be given the ShopInventory's information
        // and then UI will then need to be displayed to the player
    }
}