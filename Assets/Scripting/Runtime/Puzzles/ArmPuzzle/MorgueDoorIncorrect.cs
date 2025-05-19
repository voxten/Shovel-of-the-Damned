using UnityEngine;

public class MorgueDoorIncorrect : InteractableObject
{
    [SerializeField] private Item morgueKey;
    
    public override bool Interact()
    {
        if (Inventory.InventoryEvents.FindItem(morgueKey))
        {
            SoundManager.PlaySound3D(Sound.MorgueDoorKeyTry, transform);
            Narration.DisplayText?.Invoke("The key doesn't fit... Maybe it will work on another door...");
        }
        else
        {
            SoundManager.PlaySound3D(Sound.MorgueDoorTry, transform);
            Narration.DisplayText?.Invoke("It’s sealed tight...");
        }
        return true;
    }
}
