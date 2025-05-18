using System;
using System.Collections;
using StarterAssets;
using UnityEngine;

public class BarrierTrigger : MonoBehaviour
{
    [SerializeField] private GameObject resetLocation;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Wchodze w bariere");
            StartCoroutine(TeleportPlayer());
        }
    }

    private IEnumerator TeleportPlayer()
    {
        FirstPersonController.PlayerEvents.SetLocation(resetLocation.transform);
        yield return new WaitForSeconds(1f);
        //FirstPersonController.PlayerEvents.StopLocationChange();
    }
}
