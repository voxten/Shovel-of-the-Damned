using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlashlightOptions : MonoBehaviour
{
    [Header("Battery Settings")]
    [SerializeField] private TextMeshProUGUI showedBateryLevel;
    [SerializeField] private Item itemBattery;
    
    [Header("Light")]
    [SerializeField] private Light mainLight;
    [SerializeField] private Light lightUV;
    [SerializeField] private GameObject coneLight;
    
    [Header("Enemy")]
    [SerializeField] private EnemyAI enemyAI;
    
    private float _batterryLevel = 2.00f;
    private float _lastLowerTime;
    private bool _isEnemyInArea = false;

    private void Start()
    {
        _lastLowerTime = Time.time;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _batterryLevel > 25.00 && lightUV.intensity == 0.0f)
        {
            ActiveUV();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeBattery();
        }
        
        if (mainLight.enabled)
            LowerBatteryState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Wszedl enemy");
            _isEnemyInArea = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Wyszedl enemy");
            _isEnemyInArea = false;
        }
    }
    
    private void LowerBatteryState()
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
            mainLight.enabled = false;
            showedBateryLevel.text = "0%";
            _isEnemyInArea = false;
        }
        else
        {
            mainLight.enabled = true;
        }
    }

    private void ChangeBattery()
    {
        if (Inventory.InventoryEvents.FindItem(itemBattery) && _batterryLevel <= 25f)
        {
            Inventory.InventoryEvents.RemoveItem(itemBattery);
            _batterryLevel = 100.00f; 
            mainLight.enabled = true;
        }
    }

    private void ActiveUV()
    {
        _batterryLevel -= 25.0f;
        
        if(_isEnemyInArea)
            FearEnemy();
        
        StartCoroutine(UVLightRoutine());
    }

    private IEnumerator UVLightRoutine()
    {
        // Narastanie intensywnosci szybko
        float currentIntensity = 0f;
        float targetIntensity = 2000f;
        float riseSpeed = 3000f;

        while (currentIntensity < targetIntensity)
        {
            currentIntensity += riseSpeed * Time.deltaTime;
            currentIntensity = Mathf.Min(currentIntensity, targetIntensity);
            lightUV.intensity = currentIntensity;
            yield return null;
        }

        // Krotka chwila maksymalnego swiatla
        yield return new WaitForSeconds(2f);

        // Powolne opadanie intensywnosci
        float fallSpeed = 1000f;

        while (currentIntensity > 0)
        {
            currentIntensity -= fallSpeed * Time.deltaTime;
            currentIntensity = Mathf.Max(currentIntensity, 0);
            lightUV.intensity = currentIntensity;
            yield return null;
        }
    }
    
    private void FearEnemy()
    {
        enemyAI.FearEnemy();
    }
}