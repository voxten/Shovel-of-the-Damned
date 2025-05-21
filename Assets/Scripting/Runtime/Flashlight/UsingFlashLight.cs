using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class UsingFlashLight : MonoBehaviour
{
    [SerializeField] private GameObject flashlightObject;
    private FlashlightOptions _flashlightOptions;
    [SerializeField] private Item itemFlashlight;
    [SerializeField] private TwoBoneIKConstraint handIK;
    [SerializeField] private TwoBoneIKConstraint ShadowHandIK;
    [SerializeField] private Light mainLight;
    [SerializeField] private Light lightUV;
    [SerializeField] private GameObject textPrg;
    [SerializeField] private GameObject playerObject;
    
    public bool canActivate = true;

    private const float weightSpeed = 1.5f;
    private float handWeight = 0.0f;
    private bool _active = false;

    private void Awake()
    {
        _flashlightOptions = flashlightObject.GetComponent<FlashlightOptions>();
    }

    private void Update()
    {
        if (PlayerDeathUIPlayerDeathUIManager.DeathEvents.GetIsPlayerDead()) return;

        if (!canActivate) return;
        
        if (Input.GetKeyDown(KeyCode.F) && Inventory.InventoryEvents.FindItem(itemFlashlight) && lightUV.intensity == 0 && !NoteUIManager.NoteActions.GetIsOn() && !InventoryUIManager.InventoryUIEvents.GetIsOn())
        {
            ToggleFlashlight();
        }

        //UpdateHandIKWeight();
    }

    //private void UpdateHandIKWeight()
    //{
    //    float targetWeight = _active ? 1.0f : 0.0f;

    //    // Zmieniamy wag� wzgl�dem czasu
    //    handWeight = Mathf.MoveTowards(handWeight, targetWeight, weightSpeed * Time.deltaTime);
    //    handIK.weight = handWeight;
    //    ShadowHandIK.weight = handWeight;

    //    if (handWeight <= 0.0f && !_active)
    //    {
    //        flashlightObject.SetActive(false);
    //    }
    //}


    private void ToggleFlashlight()
    {
        if (!mainLight.enabled)
        {
            ToggleLight(true);
            SoundManager.PlaySound3D(Sound.FlashlightOn, playerObject.transform, null, 0.4f);
        }
        else
        {
            ToggleLight(false);
            SoundManager.PlaySound3D(Sound.FlashlightOff, playerObject.transform, null, 0.4f);
        }
    }

    private void ToggleLight(bool state)
    {
        _flashlightOptions.enabled = state;
        mainLight.enabled = state;
        textPrg.SetActive(state);
        _active = state;
    }
}
