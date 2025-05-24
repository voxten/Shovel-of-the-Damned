using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public List<Vent> ventPoints;
    [SerializeField] private EnemyAI enemyAI;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAI.SetCurrentZone(this);
        }
    }
}
