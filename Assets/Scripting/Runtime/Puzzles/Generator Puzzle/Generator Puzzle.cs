using System;
using UnityEngine;

public class GeneratorPuzzle : PuzzleObject
{
    [SerializeField] private Light[] onLedLights;
    [SerializeField] private Light[] batteryLedLights;
    [SerializeField] private Light[] fuelLedLights;

    private void OnEnable()
    {
        GeneratorEvents.SetBatteryLed += SetBatteryLed;
        GeneratorEvents.SetFuelLed += SetFuelLed;
    }

    private void OnDisable()
    {
        GeneratorEvents.SetBatteryLed -= SetBatteryLed;
        GeneratorEvents.SetFuelLed -= SetFuelLed;
    }

    public override void OpenPuzzle()
    {
        
    }

    public override void QuitPuzzle() 
    {
        
    }

    private void SetBatteryLed()
    {
        batteryLedLights[0].enabled = false;
        batteryLedLights[1].enabled = true;
    }

    private void SetFuelLed()
    {
        fuelLedLights[0].enabled = false;
        fuelLedLights[1].enabled = true;
    }
    
    public static class GeneratorEvents
    {
        public static Action SetBatteryLed;
        public static Action SetFuelLed;
    }
}
