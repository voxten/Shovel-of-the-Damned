using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

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
    
    private SkinnedMeshRenderer _enemySkinnedMeshRenderer;
    private Animator _enemyAnimator;
    private NavMeshAgent _enemyAgent;
    
    [Header("Main Cube Settings")]
    [SerializeField] private GameObject mainCubePrefab;
    [SerializeField] private float cubeMoveSpeed = 5f;
    [SerializeField] private float cubeRotationSpeed = 5f;
    [SerializeField] private float pointReachedThreshold = 0.1f;

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
    
    private enum AIState { MovingToVent, MovingOutVent, Searching, Chasing, Fear }
    private AIState _currentState = AIState.MovingToVent;

    private void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();
        _enemySkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _enemyAnimator = GetComponent<Animator>();
        _enemyAgent.speed = walkSpeed; // Set default speed to walk
    }

    private void Start()
    {
        SetDestination(ventPoints[0].gameObject);
        SpawnMainCube();
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
        
        while (attempts < maxAttempts)
        {
            randomVent = ventPoints[Random.Range(0, ventPoints.Length)];
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
        _currentPointIndex = 1; // Start moving to next point
        if (_cubeMovementCoroutine != null)
        {
            StopCoroutine(_cubeMovementCoroutine);
        }
        _cubeMovementCoroutine = StartCoroutine(MoveCubeThroughPoints(randomVent));
    }

    private IEnumerator MoveCubeThroughPoints(Vent vent)
    {
        while (true)
        {
            if (vent.Points.Count == 0) yield break;

            // Get current target point (looping back to 0 when reaching end)
            if (_currentPointIndex >= vent.Points.Count)
            {
                _currentPointIndex = 0;
            }

            // Skip null points
            if (vent.Points[_currentPointIndex] == null)
            {
                _currentPointIndex++;
                continue;
            }

            Transform targetPoint = vent.Points[_currentPointIndex].transform;
            
            // Move towards the point
            while (Vector3.Distance(_mainCube.transform.position, targetPoint.position) > pointReachedThreshold)
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

            // Point reached, move to next
            _currentPointIndex++;
        }
    }

    private void UpdateStateMachine()
    {
        if (_currentCoroutine != null) return; // Don't update state while a coroutine is running
        
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
                if (HasReachedDestination())
                {
                    _currentCoroutine = StartCoroutine(ClimbOutVentRoutine());
                }
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
                if (_isPlayerVisible)
                {
                    FollowPlayer();
                }
                else
                {
                    _currentState = AIState.Searching;
                    _currentSearchPointIndex = 0;
                    MoveToRandomNearbyPoint();
                }
                break;
                
            case AIState.Fear:
                _currentCoroutine = StartCoroutine(FearRoutine());
                break;
        }
        
        CheckPlayerVisibility();
    }

    private void UpdateAnimator()
    {
        _enemyAnimator.SetBool("Walk", _enemyAgent.velocity.magnitude > 0.1f && _enemyAgent.velocity.magnitude <= _enemyAgent.speed * 0.5f);
        _enemyAnimator.SetBool("Run", _enemyAgent.velocity.magnitude > _enemyAgent.speed * 0.5f);
    }

    private void CheckPlayerVisibility()
    {
        if (player == null || _isEnemyFear) return;

        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        // Check if player is within detection range
        if (distanceToPlayer <= playerDetectionRange)
        {
            // Check if player is within field of view
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            
            if (angleToPlayer <= playerDetectionAngle * 0.5f)
            {
                // Raycast to check line of sight
                if (Physics.Raycast(transform.position, directionToPlayer.normalized, out var hit, playerDetectionRange))
                {
                    Debug.Log("In Raycast");
                    _isPlayerVisible = true;
                    _currentState = AIState.Chasing;
                    return;
                }
            }
        }
        _isPlayerVisible = false;
    }

    private void TeleportEnemy(Vent vent)
    {
        transform.position = vent.transform.position;
        _enemyAgent.speed = runSpeed; // Set to run speed when coming out of vent
        _enemyAnimator.SetTrigger("ClimbOut");
        _currentState = AIState.MovingOutVent;
        SetDestination(GetRunAwayDestination());
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
        _enemyAgent.isStopped = true;
        
        // Wait for climb animation to complete
        yield return new WaitForSeconds(climbInDuration);
        
        // Hide enemy
        _enemySkinnedMeshRenderer.enabled = false;
        //TeleportEnemy(_currentVentPoint);
        
        _currentCoroutine = null;
    }

    private IEnumerator ClimbOutVentRoutine()
    {
        // Start climb out animation
        _enemyAnimator.SetTrigger("ClimbOut");
        
        // Wait for climb out animation to complete
        yield return new WaitForSeconds(climbOutDuration);
        
        // Show enemy and resume movement
        _enemySkinnedMeshRenderer.enabled = true;
        _enemyAgent.isStopped = false;
        _enemyAgent.speed = walkSpeed; // Reset to walk speed after running
        
        // Start searching
        _currentState = AIState.Searching;
        _currentSearchPointIndex = 0;
        MoveToRandomNearbyPoint();
        
        _currentCoroutine = null;
    }

    private IEnumerator FearRoutine()
    {
        _isEnemyFear = true;
        _enemyAgent.isStopped = true;
        _enemyAnimator.SetTrigger("Fear");
        
        // Wait for fear duration
        yield return new WaitForSeconds(fearDuration);
        
        // When feared, run to a nearest vent
        Vent nearestVent = FindNearestVent();
        _enemyAgent.speed = runSpeed; // Set to run speed when feared
        SetDestination(nearestVent.gameObject);
        _enemyAgent.isStopped = false;
        _currentState = AIState.MovingToVent;
        
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