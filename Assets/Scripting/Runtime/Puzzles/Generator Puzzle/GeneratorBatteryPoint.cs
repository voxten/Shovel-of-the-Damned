using System;
using UnityEngine;

public class GeneratorBatteryPoint : MonoBehaviour
{
    [SerializeField] private GameObject batteryObject;
    [SerializeField] private GameObject batteryObjectRed;
    [SerializeField] private Sound batterySound;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            GeneratorPuzzle.GeneratorEvents.SetBatteryLed();
            SoundManager.PlaySound(batterySound);
            batteryObject.SetActive(true);
            batteryObjectRed.SetActive(false);
            DragObject.DragEvents.DropObject();
            Destroy(other.gameObject);
        }
    }
}
