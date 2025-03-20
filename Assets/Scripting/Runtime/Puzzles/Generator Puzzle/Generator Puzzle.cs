using System;
using System.Collections;
using UnityEngine;

public class GeneratorPuzzle : PuzzleObject
{
    [SerializeField] private Light[] batteryLedLights;
    private bool _batteryIsOn;
    [SerializeField] private Light[] warningLedLights;
    private bool _warningIsOn;
    [SerializeField] private Light[] fuelLedLights;
    private bool _fuelIsOn;
    
    [SerializeField] private GeneratorSwitch[] switches;

    private void OnEnable()
    {
        GeneratorEvents.SetBatteryLed += SetBatteryLed;
        GeneratorEvents.CheckWarningLed += CheckWarningLed;
        GeneratorEvents.SetFuelLed += SetFuelLed;
        GeneratorEvents.CheckPuzzle += CheckPuzzle;
    }

    private void OnDisable()
    {
        GeneratorEvents.SetBatteryLed -= SetBatteryLed;
        GeneratorEvents.CheckWarningLed -= CheckWarningLed;
        GeneratorEvents.SetFuelLed -= SetFuelLed;
        GeneratorEvents.CheckPuzzle -= CheckPuzzle; 
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
        _batteryIsOn = true;
    }

    private void SetFuelLed()
    {
        fuelLedLights[0].enabled = false;
        fuelLedLights[1].enabled = true;
        _fuelIsOn = true;
    }

    private void SetWarningLed()
    {
        warningLedLights[0].enabled = false;
        warningLedLights[1].enabled = true;
        _warningIsOn = true;
    }

    private void DisableWarningLed()
    {
        warningLedLights[0].enabled = true;
        warningLedLights[1].enabled = false;
        _warningIsOn = false;
    }

    private void CheckWarningLed()
    {
        var counter = 0;
        for (var i = 0; i < switches.Length; i++)
        {
            if (switches[i].GetIsSwitched())
            {
                if (i is 0 or 2)
                {
                    counter++;
                }
                else
                {
                    counter--;
                }
            }
        }

        if (counter == 2)
        {
            SetWarningLed();
            InteractionSystem.InteractionEvents.ExitPuzzleInteraction();
        }
        else
        {
            StartCoroutine(OffLed());
        }
    }

    private bool CheckPuzzle()
    {
        if (_batteryIsOn && _warningIsOn && _fuelIsOn)
        {
            EndPuzzle();
            return true;
        }
        return false;
    }

    private IEnumerator OffLed()
    {
        SetWarningLed();
        yield return new WaitForSeconds(1);
        DisableWarningLed();
    }
    
    public static class GeneratorEvents
    {
        public static Action SetBatteryLed;
        public static Action SetFuelLed;
        public static Func<bool> CheckPuzzle;
        public static Action CheckWarningLed;
    }
}
