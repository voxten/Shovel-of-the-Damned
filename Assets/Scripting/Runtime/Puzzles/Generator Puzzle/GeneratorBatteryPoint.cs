using System;
using UnityEngine;

public class GeneratorBatteryPoint : MonoBehaviour
{
    [SerializeField] private GameObject batteryObject;
    [SerializeField] private GameObject batteryObjectRed;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            GeneratorPuzzle.GeneratorEvents.SetBatteryLed();
            SoundManager.PlaySound(Sound.GeneratorBatteryInsert);
            batteryObject.SetActive(true);
            batteryObjectRed.SetActive(false);
            DragObject.DragEvents.DropObject();
            Destroy(other.gameObject);
        }
    }
}
