using System;
using UnityEngine;
using UnityEngine.AI;

public class FinalTrigger : InteractableObject
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject enemySpawnPoint;
    private bool _isTriggered;
    private NavMeshAgent _navMeshAgent;
    private EnemyAI _enemyAI;

    private void Awake()
    {
        _navMeshAgent = enemy.GetComponent<NavMeshAgent>();
        _enemyAI = enemy.GetComponent<EnemyAI>();
    }

    public override bool Interact()
    {
        if (_isTriggered) 
            return false;
        
        //Animation player to reach hand
        
        //then spawnEnemy
        SpawnEnemy();
        
        return true;
    }

    private void SpawnEnemy()
    {
        _isTriggered = true;
        _enemyAI.StopAllCouroutines();
        _navMeshAgent.Warp(enemySpawnPoint.transform.position);
        _enemyAI.SetAttackState();
    }
}