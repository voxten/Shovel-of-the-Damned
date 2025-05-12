using System;
using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    private bool _isTriggered;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggered = false;
        }
    }

    public bool GetIsTriggered()
    {
        return _isTriggered;
    }
}
