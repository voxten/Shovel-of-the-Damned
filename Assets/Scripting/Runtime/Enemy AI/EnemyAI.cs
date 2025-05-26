using System;
using System.Collections;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private Vent[] ventPoints;
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private int searchPointsCount = 3;
    [SerializeField] private float playerDetectionRange = 15f;
    [SerializeField] private float playerDetectionAngle = 60f;
    [SerializeField] private float fearDuration = 3f;
    [SerializeField] private float climbInDuration = 1.5f;
    [SerializeField] private float climbOutDuration = 1.5f;
    
    [Header("Speed Settings")]
    [SerializeField] private float walkSpeed = 0.8f;
    [SerializeField] private float runSpeed = 1.5f;
    
    private Vent _currentVentPoint;
    private int _currentSearchPointIndex;
    private Vector3 _currentDestination;
    private Coroutine _currentCoroutine;
    
    [Header("References")]
    [SerializeField] private FirstPersonController player;
    private bool _isPlayerVisible;
    private bool _isEnemyFear;
    private bool _isPlayerKilled;

    private ZoneTrigger _currentPlayerZone;
    
    private SkinnedMeshRenderer _enemySkinnedMeshRenderer;
    private Animator _enemyAnimator;
    private NavMeshAgent _enemyAgent;

    [Header("Attack")] 
    [SerializeField] private CinemachineCamera firstAttackCamera;
    [SerializeField] private CinemachineCamera lastAttackCamera;
    
    [Header("First Encounter")]
    [SerializeField] private Vent firstVent;
    private bool _firstTime = true;
    
    [Header("Chase Settings")]
    [SerializeField] private float chaseDurationAfterLost = 5f; // Enemy chases for 5s after losing sight
    private float _chaseCooldownTimer;
    private bool _isInChaseCooldown;
    
    [Header("Main Cube Settings")]
    [SerializeField] private GameObject mainCubePrefab;
    [SerializeField] private float cubeMoveSpeed = 5f;
    [SerializeField] private float cubeRotationSpeed = 5f;
    [SerializeField] private float pointReachedThreshold = 0.1f;
    [SerializeField] private ZoneTrigger basicZoneTrigger;

    private GameObject _mainCube;
    private int _currentPointIndex;
    private Coroutine _cubeMovementCoroutine;
    
    [Header("Gizmo Settings")]
    [SerializeField] private Color searchAreaColor = new(0, 1, 0, 0.1f);
    [SerializeField] private Color detectionRangeColor = new(1, 0, 0, 0.1f);
    [SerializeField] private Color fovColor = new(1, 1, 0, 0.2f);
    [SerializeField] private Color currentPathColor = Color.blue;
    [SerializeField] private Color ventPathColor = Color.magenta;
    [SerializeField] private float gizmoLineWidth = 2f;

    private bool _isEnemyInVents;
    private Collider _collider;
    private float _lastSpeed;
    private Coroutine _stepsCoroutine;
    
    private enum AIState { MovingToVent, MovingOutVent, Searching, Chasing, Fear, Attack, Idle }
    private AIState _currentState = AIState.MovingToVent;

    private void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();
        _enemySkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _enemyAnimator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        _enemyAgent.speed = walkSpeed; // Set default speed to walk
        _lastSpeed = _enemyAgent.speed;
    }

    private void OnEnable()
    {
        _isEnemyInVents = true;
        _enemySkinnedMeshRenderer.enabled = false;
        _collider.enabled = false;

        SpawnMainCube();
        _currentCoroutine = null;
    }
    
    private void Update()
    {
        UpdateStateMachine();
        UpdateAnimator();
    }
    
    private void SpawnMainCube()
    {
        if (_mainCube != null)
        {
            Destroy(_mainCube);
        }

        // Select random vent with at least one valid point
        Vent randomVent = null;
        int attempts = 0;
        const int maxAttempts = 10;

        if (_currentPlayerZone == null)
            _currentPlayerZone = basicZoneTrigger;
        
        while (attempts < maxAttempts)
        {
            if (_firstTime)
            {
                randomVent = firstVent;
                _firstTime = false;
            }
            else
            {
                randomVent = _currentPlayerZone.ventPoints[Random.Range(0, _currentPlayerZone.ventPoints.Count)];
            }
            
            if (randomVent.Points.Count > 0 && randomVent.Points[0] != null)
            {
                break;
            }
            attempts++;
        }

        if (randomVent == null || randomVent.Points.Count == 0 || randomVent.Points[0] == null)
        {
            Debug.LogWarning("No valid vent with points found!");
            return;
        }

        // Create mainCube at first point of the vent
        _mainCube = Instantiate(mainCubePrefab, randomVent.Points[0].transform.position, Quaternion.identity);
        
        // Start moving through points
        _currentPointIndex = 0; // Start moving to next point
        if (_cubeMovementCoroutine != null)
        {
            StopCoroutine(_cubeMovementCoroutine);
        }
        _cubeMovementCoroutine = StartCoroutine(MoveCubeThroughPoints(randomVent));
    }

    public void SetAttackState(string state)
    {
        _currentCoroutine = null;
        _currentState = AIState.Attack;
        _currentCoroutine = StartCoroutine(AttackRoutine(state));
    }
    
    public void SetVentOutState(Vent vent)
    {
        _currentCoroutine = null;
        _currentVentPoint = vent;
        
        if (_cubeMovementCoroutine != null)
            StopCoroutine(_cubeMovementCoroutine);
        
        _cubeMovementCoroutine = null;
        _isEnemyInVents = false;
        _currentState = AIState.MovingOutVent;
    }

    public void SetCurrentZone(ZoneTrigger zoneTrigger)
    {
        _currentPlayerZone = zoneTrigger;
    }
    
    public void StopAllCouroutines()
    {
        _currentCoroutine = null;
        StopAllCoroutines();
    }

    public bool GetIsInVent()
    {
        return _isEnemyInVents;
    }
    
    public void SetIdleState(Vent transformPoint)
    {
        // If in vents, teleport out immediately without sounds
        if (_isEnemyInVents)
        {
            _currentVentPoint = transformPoint;
            StopCoroutine(_cubeMovementCoroutine);
            _cubeMovementCoroutine = null;
            _isEnemyInVents = false;
            TeleportEnemy(transformPoint, true); // Silent teleport
            _enemySkinnedMeshRenderer.enabled = true;
            _collider.enabled = true;
        }

        // Stop all current actions
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        // Set to idle state
        _currentState = AIState.Idle;
        _enemyAgent.isStopped = true;
        _enemyAgent.speed = 0f;

        // Stop any movement sounds
        if (_stepsCoroutine != null)
        {
            StopCoroutine(_stepsCoroutine);
            _stepsCoroutine = null;
        }
    }

    private IEnumerator MoveCubeThroughPoints(Vent vent)
    {
        if (vent.Points.Count == 0) yield break;
    
        bool movingForward = true; // Track movement direction
    
        while (true)
        {
            // Skip null points
            if (vent.Points[_currentPointIndex] == null)
            {
                _currentPointIndex += movingForward ? 1 : -1;
                continue;
            }

            Transform targetPoint = vent.Points[_currentPointIndex].transform;

            if (_mainCube == null) yield break;
        
            // Move towards the point
            while (Vector3.Distance(_mainCube.transform.position, targetPoint.position) > pointReachedThreshold && _mainCube != null)
            {
                _mainCube.transform.position = Vector3.MoveTowards(
                    _mainCube.transform.position, 
                    targetPoint.position, 
                    cubeMoveSpeed * Time.deltaTime);

                // Smoothly rotate towards movement direction
                if (_mainCube.transform.position != targetPoint.position)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetPoint.position - _mainCube.transform.position);
                    _mainCube.transform.rotation = Quaternion.Slerp(
                        _mainCube.transform.rotation, 
                        targetRotation, 
                        cubeRotationSpeed * Time.deltaTime);
                }

                yield return null;
            }

            // Determine next point based on direction
            if (movingForward)
            {
                _currentPointIndex++;
                if (_currentPointIndex >= vent.Points.Count)
                {
                    // Reached end, start moving backward
                    _currentPointIndex = vent.Points.Count - 2;
                    movingForward = false;
                }
            }
            else
            {
                _currentPointIndex--;
                if (_currentPointIndex < 0)
                {
                    // Reached start, start moving forward
                    _currentPointIndex = 1;
                    movingForward = true;
                }
            }
        }
    }

    private void UpdateStateMachine()
    {
        if (_currentCoroutine != null) return; // Don't update state while a coroutine is running

        if (_isPlayerKilled) return;

        if (_isEnemyInVents) return;
        
        if (_isInChaseCooldown && !_isPlayerVisible)
        {
            _chaseCooldownTimer -= Time.deltaTime;
            if (_chaseCooldownTimer <= 0)
            {
                Debug.Log("IsInChaseCooldown false");
                _isInChaseCooldown = false;
            }
        }
        
        if (_isEnemyFear)
        {
            if (_currentState != AIState.MovingToVent)
            {
                Vent nearestVent = FindNearestVent();
                _enemyAgent.speed = runSpeed;
                SetDestination(nearestVent.gameObject);
                _currentState = AIState.MovingToVent;
            }
        }

        switch (_currentState)
        {
            case AIState.MovingToVent:
                if (HasReachedDestination())
                {
                    _isEnemyFear = false;
                    _currentCoroutine = StartCoroutine(ClimbIntoVentRoutine());
                }
                break;
                
            case AIState.MovingOutVent:
                TeleportEnemy(_currentVentPoint);
                _currentCoroutine = StartCoroutine(ClimbOutVentRoutine());
                break;
                
            case AIState.Searching:
                if (!_isPlayerVisible && HasReachedDestination())
                {
                    if (_currentSearchPointIndex < searchPointsCount - 1)
                    {
                        _currentSearchPointIndex++;
                        MoveToRandomNearbyPoint();
                    }
                    else
                    {
                        Vent nearestVent = FindNearestVent();
                        SetDestination(nearestVent.gameObject);
                        _currentState = AIState.MovingToVent;
                    }
                }
                break;
                
            case AIState.Chasing:
                if (_isPlayerVisible || _isInChaseCooldown)
                {
                    FollowPlayer(); // Keep chasing while player is visible OR during cooldown
                }
                else
                {
                    // Player hasn't been visible for chaseDurationAfterLost seconds
                    _currentState = AIState.Searching;
                    _currentSearchPointIndex = 0;
                    MoveToRandomNearbyPoint();
                }
                break;
                
            case AIState.Fear:
                _currentCoroutine = StartCoroutine(FearRoutine());
                break;
            
            case AIState.Idle:
                // Do nothing - enemy just stands there
                break;
        }
        
        CheckPlayerVisibility();
    }

    private void UpdateAnimator()
    {
        // Update animations based on speed
        bool isMoving = _enemyAgent.velocity.magnitude > 0.1f;
        bool isRunning = Mathf.Approximately(_enemyAgent.speed, runSpeed) && isMoving;
        bool isWalking = Mathf.Approximately(_enemyAgent.speed, walkSpeed) && isMoving;

        _enemyAnimator.SetBool("Walk", isWalking);
        _enemyAnimator.SetBool("Run", isRunning);

        // Handle sound based on movement state
        if (_enemyAgent.isStopped)
        {
            // Stop all sounds when stopped
            if (_stepsCoroutine != null)
            {
                StopCoroutine(_stepsCoroutine);
                _stepsCoroutine = null;
            }
            _lastSpeed = 0;
        }
        else if (isMoving)
        {
            // Handle initial sound playback or speed changes
            if (_stepsCoroutine == null || !Mathf.Approximately(_lastSpeed, _enemyAgent.speed))
            {
                if (_stepsCoroutine != null)
                {
                    StopCoroutine(_stepsCoroutine);
                }

                if (isRunning)
                {
                    _stepsCoroutine = StartCoroutine(PlayRunSound());
                }
                else if (isWalking)
                {
                    _stepsCoroutine = StartCoroutine(PlayWalkSound());
                }

                _lastSpeed = _enemyAgent.speed;
            }
        }
    }

    private void CheckPlayerVisibility()
    {
        if (player == null || _isEnemyFear) return;

        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
    
        // Check if player is within detection range and field of view
        if (distanceToPlayer <= playerDetectionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= playerDetectionAngle * 0.5f)
            {
                // Raycast to check line of sight
                if (Physics.Raycast(transform.position, directionToPlayer.normalized, out var hit, playerDetectionRange))
                {
                    _isPlayerVisible = true;
                    _chaseCooldownTimer = chaseDurationAfterLost; // Reset timer when player is visible
                    _currentState = AIState.Chasing;
                    _isInChaseCooldown = true;
                    return;
                }
            }
        }
    
        _isPlayerVisible = false;
    }

    private void TeleportEnemy(Vent vent, bool silent = false)
    {
        transform.position = vent.transform.position;
        _enemyAnimator.SetTrigger("ClimbOut");
        if (!silent)
        {
            _enemyAgent.speed = runSpeed; // Set to run speed when coming out of vent
            _enemyAnimator.SetTrigger("ClimbOut");
            _currentState = AIState.MovingOutVent;
            SetDestination(GetRunAwayDestination());
        }
    }

    private Vector3 GetRunAwayDestination()
    {
        // Find a point away from the vent to run to
        Vector3 runDirection = (transform.position - _currentVentPoint.transform.position).normalized;
        Vector3 runDestination = transform.position + runDirection * 10f; // Run 10 units away from vent

        if (NavMesh.SamplePosition(runDestination, out var hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        
        // If sampling fails, just use a point 10 units away
        return runDestination;
    }

    private void MoveToRandomNearbyPoint()
    {
        _enemyAgent.speed = walkSpeed;
        Vector3 randomDirection = Random.insideUnitSphere * searchRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out var hit, searchRadius, NavMesh.AllAreas))
        {
            SetDestination(hit.position);
        }
    }

    private void FollowPlayer()
    {
        if (_enemyAgent.isStopped)
            _enemyAgent.isStopped = false;
    
        _enemyAgent.speed = runSpeed;
        
        SetDestination(player.gameObject);
    }

    private IEnumerator PlayRunSound()
    {
        while (true)
        {
            SoundManager.PlaySound3D(Sound.EnemyStepsRun, transform, 100);
            yield return new WaitForSeconds(3);
        }
    }
    
    private IEnumerator PlayWalkSound()
    {
        while (true)
        {
            SoundManager.PlaySound3D(Sound.EnemySteps, transform, 100);
            yield return new WaitForSeconds(1);
        }
    }
    
    private Vent FindNearestVent()
    {
        Vent nearestVent = null;
        float minDistance = float.MaxValue;
        
        foreach (Vent vent in ventPoints)
        {
            float distance = Vector3.Distance(transform.position, vent.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestVent = vent;
            }
        }
        
        return nearestVent;
    }

    private IEnumerator ClimbIntoVentRoutine()
    {
        // Start climbing animation
        _enemyAnimator.SetTrigger("ClimbIn");
        SoundManager.PlaySound3D(Sound.EnemyVentIn, transform);
        _enemyAgent.isStopped = true;
        
        // Wait for climb animation to complete
        yield return new WaitForSeconds(climbInDuration);
        
        // Hide enemy
        _isEnemyInVents = true;
        _enemySkinnedMeshRenderer.enabled = false;
        _collider.enabled = false;

        SpawnMainCube();
        _currentCoroutine = null;
    }

    private IEnumerator ClimbOutVentRoutine()
    {
        // Start climb out animation
        _enemySkinnedMeshRenderer.enabled = true;
        _enemyAnimator.SetTrigger("ClimbOut");
        
        Destroy(_mainCube);
        SoundManager.PlaySound3D(Sound.EnemyVentOut, transform);
        // Wait for climb out animation to complete
        yield return new WaitForSeconds(climbOutDuration);
        
        // Show enemy and resume movement
        
        _enemyAgent.isStopped = false;
        _enemyAgent.speed = walkSpeed; // Reset to walk speed after running
        
        // Start searching
        _currentState = AIState.Searching;
        _currentSearchPointIndex = 0;
        _collider.enabled = true;
        MoveToRandomNearbyPoint();
        
        _currentCoroutine = null;
    }

    private IEnumerator FearRoutine()
    {
        _isEnemyFear = true;
        _enemyAgent.isStopped = true;
        _enemyAnimator.SetTrigger("Fear");
        SoundManager.PlaySound3D(Sound.EnemyFear, transform);
        
        // Wait for fear duration
        yield return new WaitForSeconds(fearDuration);
        
        // When feared, run to the nearest vent
        Vent nearestVent = FindNearestVent();
        _enemyAgent.speed = runSpeed; // Set to run speed when feared
        SetDestination(nearestVent.gameObject);
        _enemyAgent.isStopped = false;
        _currentState = AIState.MovingToVent;
        
        _currentCoroutine = null;
    }
    
    private IEnumerator AttackRoutine(string state)
    {
        _enemyAgent.isStopped = true;
        _enemyAgent.speed = 0f;
        SoundManager.PlaySound3D(Sound.EnemyAttack, transform);
        _enemyAnimator.SetTrigger("Attack");
        _isPlayerKilled = true;
        FirstPersonController.PlayerEvents.ToggleMoveCamera(false);
        FirstPersonController.PlayerEvents.TogglePlayerModel();
        CameraSwitch.CameraEvents.SwitchCamera(firstAttackCamera);
        
        yield return new WaitForSeconds(1f);
        
        CameraSwitch.CameraEvents.SwitchCamera(lastAttackCamera);
        
        yield return new WaitForSeconds(1f);
        
        switch (state)
        {
            case "final":
                FinalTrigger.FinalEvents.EndGame();
                break;
            case "attack":
                PlayerDeathUIPlayerDeathUIManager.DeathEvents.KillPlayer();
                break;
        }
        _currentCoroutine = null;
    }
    
    private void SetDestination(Vector3 destination)
    {
        _currentDestination = destination;
        _enemyAgent.SetDestination(destination);
    }

    private void SetDestination(GameObject destination)
    {
        _currentDestination = destination.transform.position;
        _enemyAgent.SetDestination(destination.transform.position);
    }

    private bool HasReachedDestination()
    {
        return !_enemyAgent.pathPending 
               && _enemyAgent.remainingDistance <= _enemyAgent.stoppingDistance 
               && (!_enemyAgent.hasPath || _enemyAgent.velocity.sqrMagnitude == 0f);
    }

    public void FearEnemy()
    {
        if (_currentState == AIState.Fear) return;
        if(_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);
        _currentState = AIState.Fear;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw search area
        Gizmos.color = searchAreaColor;
        Gizmos.DrawSphere(transform.position, searchRadius);
        
        // Draw detection range
        Gizmos.color = detectionRangeColor;
        Gizmos.DrawSphere(transform.position, playerDetectionRange);
        
        // Draw field of view with thicker lines
        DrawFieldOfViewGizmo();
        
        // Draw current path if available with thicker lines
        if (_enemyAgent != null && _enemyAgent.hasPath)
        {
            Gizmos.color = _currentState == AIState.MovingToVent ? ventPathColor : currentPathColor;
            for (int i = 0; i < _enemyAgent.path.corners.Length - 1; i++)
            {
                DrawThickLine(_enemyAgent.path.corners[i], _enemyAgent.path.corners[i + 1], gizmoLineWidth);
                Gizmos.DrawSphere(_enemyAgent.path.corners[i], gizmoLineWidth * 0.1f);
            }
            Gizmos.DrawSphere(_enemyAgent.path.corners[^1], gizmoLineWidth * 0.15f);
        }
        
        // Draw current destination with bigger marker
        if (_currentState is AIState.Searching or AIState.Chasing or AIState.MovingOutVent)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_currentDestination, Vector3.one * gizmoLineWidth * 0.5f);
        }
    }

    private void DrawFieldOfViewGizmo()
    {
        Gizmos.color = fovColor;
        float halfAngle = playerDetectionAngle * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);
        
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        
        DrawThickLine(transform.position, transform.position + leftRayDirection * playerDetectionRange, gizmoLineWidth);
        DrawThickLine(transform.position, transform.position + rightRayDirection * playerDetectionRange, gizmoLineWidth);
        
        // Draw the arc with thicker lines
        Vector3 prevPoint = transform.position + leftRayDirection * playerDetectionRange;
        for (int i = 0; i <= 20; i++)
        {
            float angle = -halfAngle + (playerDetectionAngle * i / 20);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            var nextPoint = transform.position + (rot * transform.forward) * playerDetectionRange;
            DrawThickLine(prevPoint, nextPoint, gizmoLineWidth);
            prevPoint = nextPoint;
        }
        DrawThickLine(prevPoint, transform.position + rightRayDirection * playerDetectionRange, gizmoLineWidth);
    }

    // Helper method to draw thicker lines for gizmos
    private void DrawThickLine(Vector3 start, Vector3 end, float width)
    {
        Camera c = Camera.current;
        if (c == null) return;

        // Calculate perpendicular vector to the line in screen space
        Vector3 startPos = c.WorldToScreenPoint(start);
        Vector3 endPos = c.WorldToScreenPoint(end);
        Vector3 direction = (endPos - startPos).normalized;
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0f) * width * 0.5f;

        // Draw multiple lines to simulate thickness
        Gizmos.DrawLine(c.ScreenToWorldPoint(startPos + perpendicular), c.ScreenToWorldPoint(endPos + perpendicular));
        Gizmos.DrawLine(c.ScreenToWorldPoint(startPos - perpendicular), c.ScreenToWorldPoint(endPos - perpendicular));
        Gizmos.DrawLine(start, end);
    }
}