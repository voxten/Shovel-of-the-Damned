using System.Collections;
using TMPro;
using UnityEngine;

public class FlashlightOptions : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI showedBateryLevel;
    [SerializeField] private Item itemBattery;
    [SerializeField] private GameObject light;
    [SerializeField] private Light lightUV;
    private float _batterryLevel = 2.00f;
    private float _lastLowerTime;

    private void Start()
    {
        _lastLowerTime = Time.time;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _batterryLevel>25.00 && lightUV.intensity == 0.0f)
        {
            ActiveUV();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeBattery();
        }
        LowerBateryState();
    }


    private void LowerBateryState()
    {
        if (Time.time - _lastLowerTime > 0.1f)
        {
            _lastLowerTime = Time.time;
            _batterryLevel -= 0.05f;

        }
        int toShow = System.Convert.ToInt32(System.Math.Floor(_batterryLevel));
        showedBateryLevel.text = toShow + "%";
        if (_batterryLevel < 0.0)
        {
            light.SetActive(false);
            showedBateryLevel.text = "0%";
        }
        else
        {
            light.SetActive(true);
        }
    }

    private void ChangeBattery()
    {
        if (Inventory.InventoryEvents.FindItem(itemBattery) && _batterryLevel <= 25f)
        {
            Inventory.InventoryEvents.RemoveItem(itemBattery);
            _batterryLevel = 100.00f; 
            light.SetActive(true);
        }
    }

    private void ActiveUV()
    {
        _batterryLevel -= 25.0f;
        StartCoroutine(UVLightRoutine());
    }

    private IEnumerator UVLightRoutine()
    {
        // Narastanie intensywnoœci szybko
        float currentIntensity = 0f;
        float targetIntensity = 2000f;
        float riseSpeed = 3000f; // Jak szybko narasta œwiat³o

        while (currentIntensity < targetIntensity)
        {
            currentIntensity += riseSpeed * Time.deltaTime;
            currentIntensity = Mathf.Min(currentIntensity, targetIntensity); // ¿eby nie przekroczyæ
            lightUV.intensity = currentIntensity;
            yield return null; // czekaj do nastêpnej klatki
        }

        // Krótka chwila maksymalnego œwiat³a
        yield return new WaitForSeconds(2f);

        // Powolne opadanie intensywnoœci
        float fallSpeed = 1000f; // Jak wolno spada œwiat³o

        while (currentIntensity > 0)
        {
            currentIntensity -= fallSpeed * Time.deltaTime;
            currentIntensity = Mathf.Max(currentIntensity, 0); // ¿eby nie zejœæ poni¿ej 0
            lightUV.intensity = currentIntensity;
            yield return null;
        }
    }
}
