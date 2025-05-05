using System;
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

    private const float weightSpeed = 1.5f;
    private float handWeight = 0.0f;
    private bool _active = false;

    private void Awake()
    {
        _flashlightOptions = flashlightObject.GetComponent<FlashlightOptions>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Inventory.InventoryEvents.FindItem(itemFlashlight) && lightUV.intensity == 0)
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
            _flashlightOptions.enabled = true;
            mainLight.enabled = true;
            textPrg.SetActive(true);
            SoundManager.PlaySound3D(Sound.FlashlightOn, transform);
            _active = true;
        }
        else
        {
            _flashlightOptions.enabled = false;
            mainLight.enabled = false;
            textPrg.SetActive(false);
            SoundManager.PlaySound3D(Sound.FlashlightOff, transform);
            _active = false;
        }
    }
}
