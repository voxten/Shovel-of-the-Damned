using System;
using UnityEngine;

public class PickItem : InteractableObject
{
    [SerializeField] protected Item item;
    [SerializeField] private Sound pickSound;
    public bool isPickable;
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
            SoundManager.PlaySound3D(pickSound, transform);
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
