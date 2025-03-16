using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Item> items = new();

    private void OnEnable()
    {
        InventoryEvents.GetItemsByType += GetItemsByType;
        InventoryEvents.RemoveItem += RemoveItem;
        InventoryEvents.RemoveItems += RemoveItems;
        InventoryEvents.AddItem += AddItem;
        InventoryEvents.FindItem += FindItem;
        InventoryEvents.FindItems += FindItems;
    }

    private void OnDisable()
    {
        InventoryEvents.GetItemsByType -= GetItemsByType;
        InventoryEvents.RemoveItem -= RemoveItem;
        InventoryEvents.RemoveItems -= RemoveItems;
        InventoryEvents.AddItem -= AddItem;
        InventoryEvents.FindItem -= FindItem;
        InventoryEvents.FindItems -= FindItems;
    }
    
    private void AddItem(Item item)
    {
        items.Add(item);
    }
    
    private bool RemoveItem(Item itemToRemove)
    {
        foreach (var item in items)
        {
            if (item.id == itemToRemove.id)
            {
                items.Remove(item);
                return true;
            }
        }
        return false;
    }
    
    private bool RemoveItems(Item itemToRemove, int count)
    {
        if (FindItems(itemToRemove, count))
        {
            foreach (var item in items)
            {
                if (item.id == itemToRemove.id)
                {
                    items.Remove(item);
                }
            }
            return true;
        }
        return false;
    }

    private bool FindItem(Item itemToFind)
    {
        foreach (var item in items)
        {
            if (item.id == itemToFind.id)
            {
                return true;
            }
        }
        return false;
    }
    
    private bool FindItems(Item itemToFind, int count)
    {
        var currentCount = 0;
        foreach (var item in items)
        {
            if (item.id == itemToFind.id)
            {
                currentCount++;
            }
        }
        return currentCount == count;
    }

    private List<Item> GetItemsByType(ItemType itemType)
    {
        List<Item> itemsTemp = new();
        foreach (var item in items)
        {
            if (item.itemType == itemType)
            {
                itemsTemp.Add(item);
            }
        }
        return itemsTemp;
    }

    public static class InventoryEvents
    {
        public static Action<Item> AddItem;
        public static Func<ItemType, List<Item>> GetItemsByType;
        public static Func<Item, bool> RemoveItem;
        public static Func<Item, int, bool> RemoveItems;
        public static Func<Item, bool> FindItem;
        public static Func<Item, int, bool> FindItems;
    }
}

