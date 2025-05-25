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

    public bool inPuzzle;

    public float batteryLevel;
    private float _lastLowerTime;
    private bool _isEnemyInArea;
    
    [SerializeField] private TutorialObject tutorialObject;
    public bool tutorialCompleted;
    public bool isOnce;
    
    [SerializeField] private UsingFlashLight usingFlashLight;

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
        showedBateryLevel.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        mainLight.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && batteryLevel > 25.00 && lightUV.intensity == 0.0f && !inPuzzle)
        {
            ActiveUV();
        }

        if (Input.GetKeyDown(KeyCode.R) && usingFlashLight.canActivate)
        {
            ChangeBattery();
        }

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
            batteryLevel -= 0.02f;
        }
        
        int toShow = Convert.ToInt32(Math.Floor(batteryLevel));
        showedBateryLevel.text = toShow + "%";
        
        if (batteryLevel <= 0.0)
        {
            mainLight.enabled = false;
            showedBateryLevel.text = "0%";
            _isEnemyInArea = false;
        }
        else
        {
            mainLight.enabled = true;
        }

        if (tutorialCompleted) return;

        if (batteryLevel < 25.00 && !isOnce)
        {
            isOnce = true;
            TutorialManager.TutorialManagerEvents.startTutorial(tutorialObject);
        }
    }

    private void ChangeBattery()
    {
        if (Inventory.InventoryEvents.FindItem(itemBattery) && batteryLevel <= 25f)
        {
            SoundManager.PlaySound3D(Sound.FlashlightBattery, transform, null, 0.4f);
            Inventory.InventoryEvents.RemoveItem(itemBattery);
            tutorialCompleted = true;
            TutorialManager.TutorialManagerEvents.stopTutorial();
            batteryLevel = 100.00f; 
            mainLight.enabled = true;
        }
    }

    private void ActiveUV()
    {
        batteryLevel -= 25.0f;
        
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