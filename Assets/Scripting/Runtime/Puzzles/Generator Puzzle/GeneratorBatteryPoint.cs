using System;
using UnityEngine;

public class GeneratorBatteryPoint : MonoBehaviour
{
    [SerializeField] private GameObject batteryObject;
    [SerializeField] private GameObject batteryObjectRed;

    private void Start()
    {
        GeneratorPuzzle generatorPuzzle = FindObjectOfType<GeneratorPuzzle>();
        if (generatorPuzzle.isFinished && generatorPuzzle.isAfterLoad)
        {
            GeneratorPuzzle.GeneratorEvents.SetBatteryLed();
            batteryObject.SetActive(true);
            batteryObjectRed.SetActive(false);
        }
    }
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
