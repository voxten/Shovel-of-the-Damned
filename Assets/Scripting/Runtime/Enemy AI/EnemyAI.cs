using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Vent[] ventPoints;
    private Vent _currentVentPoint;
    
    private SkinnedMeshRenderer _enemySkinnedMeshRenderer;
    private Animator _enemyAnimator;
    private NavMeshAgent _enemyAgent;
    
    [SerializeField] private FirstPersonController player;
    
    private void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();
        _enemySkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _enemyAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        SetDestination(ventPoints[0].gameObject);
    }
    
    // only for testing
    public void CheckVent(Vent vent)
    {
        ClimbToVent();
    }

    private void TeleportEnemy(Vent vent)
    {
        vent.transform.position = player.transform.position;
        _enemyAnimator.SetTrigger("ClimbOut");
        
        //Poczekaj na koniec wyskoku z ventu, zacznij przeszukiwać
        SearchEnvironment();
    }

    private void ClimbToVent()
    {
        _enemyAnimator.SetTrigger("ClimbIn");
        _enemySkinnedMeshRenderer.enabled = false;
    }

    private void ClimbOutVent()
    {
        TeleportEnemy(_currentVentPoint);
        _enemyAnimator.SetTrigger("ClimbOut");
        _enemySkinnedMeshRenderer.enabled = true;
    }

    private void SearchEnvironment()
    {
        _enemyAnimator.SetTrigger("Search");
        MoveAround();
    }

    private void MoveAround()
    {
        // SetDestination somewhere on mesh surface that is next to him and move there do it few times then go to the nearest vent
    }

    private void FollowPlayer()
    {
        // When player is found (by using raycast) follow him
        if(_enemyAgent.isStopped)
            _enemyAgent.isStopped = false;
        
        _enemyAnimator.SetBool("Run", true);
        _enemyAnimator.SetBool("Walk", true);
        _enemyAgent.SetDestination(player.transform.position);
        SetDestination(player.gameObject);
    }

    private void GetHitByPlayer()
    {
        _enemyAgent.isStopped = true;
        _enemyAnimator.SetTrigger("Fear");
        _enemyAnimator.SetBool("Walk", false);
        _enemyAnimator.SetBool("Run", false);
        
        //Wait until Fear is ended
        //Go to the nearest Vent
    }

    private void SetDestination(GameObject destination)
    {
        _enemyAgent.SetDestination(destination.transform.position);
    }
}
