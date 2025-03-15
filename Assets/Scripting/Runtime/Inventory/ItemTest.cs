using System;
using UnityEngine;

public class ItemTest : MonoBehaviour
{
    [SerializeField] private Item item;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered");
            Inventory.InventoryEvents.AddItem(item);
        }
    }
}
