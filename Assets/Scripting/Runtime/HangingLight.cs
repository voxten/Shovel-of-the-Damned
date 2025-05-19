using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HangingLight : MonoBehaviour
{
    [SerializeField] private List<Light> spotLights;
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity = 1f;

    [SerializeField] private bool shouldFlick;
    
    [Header("Flicker Settings")]
    [SerializeField] private float minFlickerDelay = 0.05f;
    [SerializeField] private float maxFlickerDelay = 0.2f;
    [SerializeField] private float minFlickerDuration = 0.1f;
    [SerializeField] private float maxFlickerDuration = 0.5f;
    [SerializeField] private float minNormalDuration = 0.5f;
    [SerializeField] private float maxNormalDuration = 3f;
    [SerializeField] private float chanceForDoubleFlicker = 0.3f;

    [Header("Sound Settings")]
    [SerializeField] private bool loopSound = true;

    private Coroutine _flickerRoutine;
    private Coroutine _soundRoutine;
    private bool _isFlickering;
    
    private void Start()
    {
        if (loopSound)
        {
            SoundManager.PlaySound3D(Sound.HangingLightLoop, transform, null, 0.2f);
        }

        if (spotLights[0] != null && shouldFlick)
            StartCoroutine(FlickerLoop());
    }

    private IEnumerator FlickerLoop()
    {
        spotLights[0].gameObject.SetActive(spotLights[0].gameObject.activeSelf);
        while (true)
        {
            var normalWait = Random.Range(minNormalDuration, maxNormalDuration);
            yield return new WaitForSeconds(normalWait);
            
            _flickerRoutine = StartCoroutine(FlickerSequence());
            
            while (_isFlickering)
                yield return null;
        }
    }

    private IEnumerator FlickerSequence()
    {
        _isFlickering = true;
        
        var flickerCount = Random.value < chanceForDoubleFlicker ? 2 : 1;

        for (var i = 0; i < flickerCount; i++)
        {
            SetLightsIntensity(minIntensity);
            
            var flickerDuration = Random.Range(minFlickerDuration, maxFlickerDuration);
            yield return new WaitForSeconds(flickerDuration);
            
            SetLightsIntensity(maxIntensity);
            
            if (i < flickerCount - 1)
            {
                var flickerDelay = Random.Range(minFlickerDelay, maxFlickerDelay);
                yield return new WaitForSeconds(flickerDelay);
            }
        }
        
        _isFlickering = false;
    }

    private void SetLightsIntensity(float intensity)
    {
        foreach (var light in spotLights)
        {
            light.intensity = intensity;
        }
    }

    private void OnDestroy()
    {
        if (_flickerRoutine != null)
            StopCoroutine(_flickerRoutine);
            
        if (_soundRoutine != null)
            StopCoroutine(_soundRoutine);
    }
}