using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour
{
    [SerializeField] private List<Item> items;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered");
            AddItems();
        }
    }

    private void AddItems()
    {
        foreach (var item in items)
        {
            Inventory.InventoryEvents.AddItem(item);
        }
    }
}
