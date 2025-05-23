using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

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
    
    private float _batteryLevel = 100.00f;
    private float _lastLowerTime;
    private bool _isEnemyInArea;
    
    [SerializeField] private TutorialObject tutorialObject;
    private bool _tutorialCompleted;
    private bool _isOnce;

    private void Start()
    {
        _lastLowerTime = Time.time;
    }

    public static class FlashlightOptionsEvents
    {
        public static Func<float> GetBatteryLevel;
        public static Action<float> SetBatteryLevel;
    }
    private void OnEnable()
    {
        FlashlightOptionsEvents.GetBatteryLevel += GetBatteryLevel;
        FlashlightOptionsEvents.SetBatteryLevel += SetBatteryLevel;
    }

    private void OnDisable()
    {
        FlashlightOptionsEvents.GetBatteryLevel -= GetBatteryLevel;
        FlashlightOptionsEvents.SetBatteryLevel -= SetBatteryLevel;
    }

    private float GetBatteryLevel()
    {
        return _batterryLevel;
    }
    private void SetBatteryLevel(float batteryLevel)
    {
        _batterryLevel = batteryLevel;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _batteryLevel > 25.00 && lightUV.intensity == 0.0f)
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
            _batteryLevel -= 0.05f;
        }
        
        int toShow = System.Convert.ToInt32(System.Math.Floor(_batteryLevel));
        showedBateryLevel.text = toShow + "%";
        
        if (_batteryLevel < 0.0)
        {
            mainLight.enabled = false;
            showedBateryLevel.text = "0%";
            _isEnemyInArea = false;
        }
        else
        {
            mainLight.enabled = true;
        }

        if (_tutorialCompleted) return;

        if (_batteryLevel < 5.00 && !_isOnce)
        {
            _isOnce = true;
            TutorialManager.TutorialManagerEvents.startTutorial(tutorialObject);
        }
    }

    private void ChangeBattery()
    {
        if (Inventory.InventoryEvents.FindItem(itemBattery) && _batteryLevel <= 5f)
        {
            Inventory.InventoryEvents.RemoveItem(itemBattery);
            _tutorialCompleted = true;
            TutorialManager.TutorialManagerEvents.stopTutorial();
            _batteryLevel = 100.00f; 
            mainLight.enabled = true;
        }
    }

    private void ActiveUV()
    {
        _batteryLevel -= 25.0f;
        
        if(_isEnemyInArea)
            FearEnemy();
        
        StartCoroutine(UVLightRoutine());
    }

    private IEnumerator UVLightRoutine()
    {
        // Narastanie intensywnosci szybko
        float currentIntensity = 0f;
        float targetIntensity = 2000f;
        float riseSpeed = 5000f;

        while (currentIntensity < targetIntensity)
        {
            currentIntensity += riseSpeed * Time.deltaTime;
            currentIntensity = Mathf.Min(currentIntensity, targetIntensity);
            lightUV.intensity = currentIntensity;
            yield return null;
        }

        // Krotka chwila maksymalnego swiatla
        yield return new WaitForSeconds(1f);

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