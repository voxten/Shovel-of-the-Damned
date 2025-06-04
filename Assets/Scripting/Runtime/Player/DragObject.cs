using System;
using StarterAssets;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    [SerializeField] private float pickupRange = 5.0f; // Initial range within which you can pick up objects
    [SerializeField] private float moveForce = 250.0f; // Force applied to move the object
    [SerializeField] private float rotationSpeed = 100.0f; // Speed for rotating the object
    [SerializeField] private float doorRotationSpeed; // Speed for rotating the object
    [SerializeField] private float throwForceMultiplier = 10.0f; // Multiplier for the throw force
    [SerializeField] private float rotationThrowForce = 10.0f; // Multiplier for the rotational throw force
    [SerializeField] private LayerMask moveableLayer; // Layer for interactable objects
    private Camera _playerCamera;
    private Rigidbody _pickedObject;
    private bool _isDragging;
    private bool _isRotatingDoor;
    
    private float _rotationX;
    private float _rotationY;
    private float _rotationZ;

    private Vector3 _lastMousePosition;
    private Vector3 _mouseDelta;
    
    private bool _isObjectMoving;

    private bool _firstTime = true;

    private void Start()
    {
        _playerCamera = Camera.main;
    }

    private void OnEnable()
    {
        DragEvents.GetDragging += GetDragging;
        DragEvents.DropObject += DropObject;
    }

    private void OnDisable()
    {
        DragEvents.GetDragging -= GetDragging;
        DragEvents.DropObject -= DropObject;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPickObject();
        }

        if (Input.GetMouseButton(0) && _isDragging && !PauseManager.PauseEvents.GetIsOn())
        {
            // Check if object is still moving
            _isObjectMoving = Vector3.Distance(_pickedObject.position,
                _playerCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2))) > 0.1f;
            
            if (_firstTime)
            {
                TutorialManager.TutorialManagerEvents.startRotateTutorial();
                _firstTime = false;
            }
            
            if (Input.GetKey(KeyCode.H) && !_isObjectMoving)
            {
                // Right-click for rotation (only if the object is not moving)
                FirstPersonController.PlayerEvents.ToggleMoveCamera(false);
                _pickedObject.isKinematic = true;
                UpdateRotation();
            }
            else
            {
                // Allow normal dragging movement
                FirstPersonController.PlayerEvents.ToggleMoveCamera(true);
                _pickedObject.isKinematic = false;
                MoveObject();
            }
        }

        if (Input.GetMouseButtonUp(0) && _isDragging && !PauseManager.PauseEvents.GetIsOn())
        {
            DropObject();
        }

        // Calculate mouse delta for throw force
        if (_isDragging)
        {
            _mouseDelta = Input.mousePosition - _lastMousePosition;
            _lastMousePosition = Input.mousePosition;
        }
        else
        {
            _lastMousePosition = Input.mousePosition;
            _mouseDelta = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (_isDragging && _pickedObject.gameObject.layer != LayerMask.NameToLayer("Drawer"))
        {
            MoveObject();
        }
    }

    private void TryPickObject()
    {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, moveableLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                InteractionSystem.InteractionEvents.DisableInteractionIcon();
                _pickedObject = rb;
                _pickedObject.useGravity = false;
                _pickedObject.maxLinearVelocity = 3;
                _isDragging = true;

                // Capture initial rotation
                Vector3 initialRotation = _pickedObject.rotation.eulerAngles;
                _rotationX = initialRotation.y;
                _rotationY = initialRotation.x;

                _lastMousePosition = Input.mousePosition; // Initialize last mouse position
            }
        }
    }

    private void DropObject()
    {
        if (_pickedObject != null)
        {
            Outline outline = _pickedObject.GetComponent<Outline>();
            if (outline != null)
            {
                Destroy(outline);
            }

            // Apply throw force
            Vector3 throwForce = _mouseDelta * throwForceMultiplier;
            //pickedObject.linearVelocity = playerCamera.transform.forward * throwForce.z;

            _pickedObject.AddTorque(_playerCamera.transform.right * -_mouseDelta.y * rotationThrowForce);
            _pickedObject.AddTorque(_playerCamera.transform.up * -_mouseDelta.x * rotationThrowForce);

            _pickedObject.useGravity = true;
            _pickedObject.isKinematic = false; // Ensure physics interactions are restored
            FirstPersonController.PlayerEvents.ToggleMoveCamera(true);
            _pickedObject.maxLinearVelocity = 0.7f;
            _pickedObject = null;
            _isRotatingDoor = false;
        }

        _isDragging = false;
        DragEvents.ObjectDropped?.Invoke();
    }

    private void MoveObject()
    {
        InteractionSystem.InteractionEvents.DisableInteractionIcon();
        // Get the custom interaction point if available
        Vector3 point = _pickedObject.GetComponent<CustomObjectPoint>()?.GetCustomPoint() ?? _pickedObject.position;

        Vector3 targetPosition = _playerCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2));
        Vector3 forceDirection = targetPosition - point;

        _isObjectMoving = forceDirection.magnitude > 0.1f;
        if (_isObjectMoving)
        {
            _pickedObject.isKinematic = false;
        }

        _pickedObject.linearVelocity = forceDirection * moveForce * Time.fixedDeltaTime;
    }
    
    private void UpdateRotation()
    {
        // Rotate with mouse movement (X and Y axes)
        _rotationX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        _rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        // Rotate with scroll wheel (Z-axis rotation)
        _rotationZ += Input.GetAxis("Mouse ScrollWheel") * rotationSpeed * 1000 * Time.deltaTime;

        // Apply the new rotation
        Quaternion targetRotation = Quaternion.Euler(_rotationY, _rotationX, _rotationZ);
        _pickedObject.MoveRotation(targetRotation);
    }
    
    private bool GetDragging()
    {
        return _isDragging;
    }

    public static class DragEvents
    {
        public static Func<bool> GetDragging;
        public static Action DropObject;
        public static Action ObjectDropped;
    }
}