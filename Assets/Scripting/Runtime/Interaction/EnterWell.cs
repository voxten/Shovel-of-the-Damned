using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterWell : InteractableObject
{
    [SerializeField] private Item flashlight;
    [SerializeField] private Item battery;
    
    public override bool Interact()
    {
        if (Inventory.InventoryEvents.FindItem(flashlight) && Inventory.InventoryEvents.FindItem(battery))
        {
            SceneLoader.SceneEvents.AnimateLoadScene("MainScene");
        }
        else
        {
            Narration.DisplayText?.Invoke("I need to grab a flashlight and the battery out of the car.");
        }
        return true;
    }
}
