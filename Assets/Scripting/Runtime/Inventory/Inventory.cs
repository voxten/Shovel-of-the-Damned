using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Item> items = new();
    [SerializeField] private Sprite noteIcon;
    [SerializeField] private Sprite backpackIcon;
    [SerializeField] private Image iconImage;
    [SerializeField] private bool shouldGetStartupItems;
    [SerializeField] private List<Item> startupItems;

    private void OnEnable()
    {
        InventoryEvents.GetAccessCard += GetAccessCard;
        InventoryEvents.GetItemsByType += GetItemsByType;
        InventoryEvents.GetAllItems += GetAllItems;
        InventoryEvents.RemoveItem += RemoveItem;
        InventoryEvents.RemoveItems += RemoveItems;
        InventoryEvents.AddItem += AddItem;
        InventoryEvents.FindItem += FindItem;
        InventoryEvents.FindItems += FindItems;
    }

    private void OnDisable()
    {
        InventoryEvents.GetAccessCard -= GetAccessCard;
        InventoryEvents.GetItemsByType -= GetItemsByType;
        InventoryEvents.GetAllItems -= GetAllItems;
        InventoryEvents.RemoveItem -= RemoveItem;
        InventoryEvents.RemoveItems -= RemoveItems;
        InventoryEvents.AddItem -= AddItem;
        InventoryEvents.FindItem -= FindItem;
        InventoryEvents.FindItems -= FindItems;
    }

    private void Awake()
    {
        if (!shouldGetStartupItems) return;
        
        foreach (var item in startupItems)
        {
            items.Add(item);
        }
    }
    
    private void AnimateIcon()
    {
        if (iconImage == null) return;

        iconImage.DOFade(1f, 0.5f).OnComplete(() =>  // Ensure it starts visible
        {
            Sequence sequence = DOTween.Sequence();

            for (var i = 0; i < 3; i++)
            {
                sequence.Append(iconImage.DOFade(0f, 0.3f)).Append(iconImage.DOFade(1f, 0.3f));
            }

            sequence.Append(iconImage.DOFade(0f, 0.3f)).Play();
        });
    }
    
    private void AddItem(Item item)
    {
        items.Add(item);
        iconImage.sprite = item is NoteItem ? noteIcon : backpackIcon;
        AnimateIcon();
    }
    
    private bool RemoveItem(Item itemToRemove)
    {
        foreach (var item in items)
        {
            if (item.Id == itemToRemove.Id)
            {
                items.Remove(item);
                return true;
            }
        }
        return false;
    }
    
    private bool RemoveItems(Item itemToRemove, int count)
    {
        var currentCount = 0;
        if (FindItems(itemToRemove, count))
        {
            foreach (var item in items)
            {
                if (currentCount == count)
                {
                    return true;
                }
                
                if (item.Id == itemToRemove.Id)
                {
                    currentCount++;
                    items.Remove(item);
                }
            }
        }
        return false;
    }

    private bool FindItem(Item itemToFind)
    {
        foreach (var item in items)
        {
            if (item.Id == itemToFind.Id)
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
            if (item.Id == itemToFind.Id)
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
    
    private Item GetAccessCard()
    {
        foreach (var item in items)
        {
            if (item is AccessCardItem)
            {
                return item;
            }
        }
        return null;
    }

    private List<Item> GetAllItems()
    {
        return items;
    }

    public static class InventoryEvents
    {
        public static Func<Item> GetAccessCard;
        public static Action<Item> AddItem;
        public static Func<ItemType, List<Item>> GetItemsByType;
        public static Func<List<Item>> GetAllItems;
        public static Func<Item, bool> RemoveItem;
        public static Func<Item, int, bool> RemoveItems;
        public static Func<Item, bool> FindItem;
        public static Func<Item, int, bool> FindItems;
    }
}

