using System;
using System.Collections.Generic;
using UnityEngine;

public class EnterCave : MonoBehaviour
{
    [SerializeField] private List<Light> lights;
    [SerializeField] private List<GameObject> lightObjects;
    [SerializeField] private DoorTrigger doorTrigger;
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private Vent vent;
    private bool _triggered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        
        doorTrigger.canOpen = false;
        if (other.CompareTag("Player"))
        {
            foreach (var lObject in lightObjects)
            {
                SoundManager.PlaySound3D(Sound.IndustrialLightOff, lObject.transform, null, 0.8f);
            }
            
            foreach (var lightd in lights)
            {
                lightd.enabled = false;
            }
        }

        enemyAI.SetIdleState(vent);
        _triggered = true;
    }
}
