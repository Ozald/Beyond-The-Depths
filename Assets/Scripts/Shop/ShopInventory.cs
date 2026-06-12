using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An item in a shop
/// </summary>
[Serializable]
public struct ShopItem
{
    // The type may need to be changed
    public GameObject item;
    public int cost;
    public int quantity;
}

/// <summary>
/// The inventory for a shop. Needs to be
/// attached to a UI.
/// </summary>
public class ShopInventory : MonoBehaviour
{
    public List<ShopItem> items;

    /// <summary>
    /// When the player purchases an item.
    /// No refunds.
    /// </summary>
    /// <param name="player">
    /// The purchasing player
    /// </param>
    /// <param name="item">
    /// The item being purchased
    /// </param>
    /// <returns>
    /// if the purchase should be successful
    /// </returns>
    public bool Purchase(StatsManager player, ShopItem item)
    {
        if (item.quantity <= 0)
            return false;
        
        // Somewhere, maybe not here, we have to actually give the player the item.
        // Alternatively, the shops could scam the player and not give anything while
        // still taking money.
        if (player.doubloons >= item.cost)
        {
            item.quantity--;
            return true;
        }

        return false;
    }
}