using UnityEngine;

public class MorgueDoorIncorrect : InteractableObject
{
    [SerializeField] private Item morgueKey;
    
    public override bool Interact()
    {
        if (Inventory.InventoryEvents.FindItem(morgueKey))
        {
            Narration.DisplayText?.Invoke("The key doesn't fit... Maybe it will work on another door...");
        }
        else
        {
            Narration.DisplayText?.Invoke("It’s sealed tight...");
        }
        return true;
    }
}
