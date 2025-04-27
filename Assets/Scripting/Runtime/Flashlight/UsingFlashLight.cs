using UnityEngine;
using UnityEngine.Animations.Rigging;

public class UsingFlashLight : MonoBehaviour
{
    [SerializeField] private GameObject flashlightObject;
    [SerializeField] private Item itemFlashlight;
    [SerializeField] private TwoBoneIKConstraint handIK;
    [SerializeField] private TwoBoneIKConstraint ShadowHandIK;
    [SerializeField] private GameObject light;
    [SerializeField] private Light lightUV;
    [SerializeField] private GameObject textPrg;

    private const float weightSpeed = 1.5f;

    private float handWeight = 0.0f;

    private bool _active = false;
    void Update()
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

    //    // Zmieniamy wagê wzglêdem czasu
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
        if (!flashlightObject.activeSelf)
        {
            flashlightObject.SetActive(true);
            textPrg.SetActive(true);
            _active = true;
        }
        else
        {
            flashlightObject.SetActive(false);
            _active = false;
            light.SetActive(false);
            textPrg.SetActive(false);
        }
    }
}
