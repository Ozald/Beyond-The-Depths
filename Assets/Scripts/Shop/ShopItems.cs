using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SellableItem
{
    public Interactable item;
    public int price;
    public int weight;
}

[CreateAssetMenu(fileName = "Shop Items", menuName = "ScriptableObjects/ShopItem", order = 1)]
public class ShopItems : ScriptableObject
{
    public List<SellableItem> items;

    public SellableItem GetItem()
    {
        ItemSelector<SellableItem> itemSelector = new ItemSelector<SellableItem>();
        
        foreach (SellableItem item in items)
            itemSelector.AddItem(item, item.weight);

        return itemSelector.Roll();
    }
}
