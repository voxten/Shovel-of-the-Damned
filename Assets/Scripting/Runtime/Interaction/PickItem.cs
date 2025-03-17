using System;
using UnityEngine;

public class PickItem : InteractableObject
{
    [SerializeField] private Item item;
    private bool _isFirst = true;
    private Collider _collider;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    public override bool Interact()
    {
        if (_isFirst)
        {
            base.Interact();
            Inventory.InventoryEvents.AddItem(item);
            gameObject.SetActive(false);
            _isFirst = false;
            _collider.enabled = false;
        }
        return true;
    }
}
