using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerNarration : MonoBehaviour {
    [SerializeField] private string message;
    [SerializeField] private bool oneTime;

    private bool triggered;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if(triggered) return;
            Narration.DisplayText?.Invoke(message);
            triggered = true;
        }
    }
}
