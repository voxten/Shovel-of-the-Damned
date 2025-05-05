using System;
using UnityEngine;

public class PickItem : InteractableObject
{
    [SerializeField] protected Item item;
    public bool isPickable = true;
    private bool _isFirst = true;
    private Collider _collider;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    public override bool Interact()
    {
        if (_isFirst && isPickable)
        {
            base.Interact();
            SoundManager.PlaySound3D(Sound.Pickup, transform);
            Inventory.InventoryEvents.AddItem(item);
            gameObject.SetActive(false);
            _isFirst = false;
            _collider.enabled = false;
            if (item is NoteItem note)
            {
                NoteUIManager.NoteActions.OpenNote(note);
            }
        }
        return true;
    }
}
