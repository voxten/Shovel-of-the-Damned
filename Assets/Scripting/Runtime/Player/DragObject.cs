using System;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    public float pickupRange = 5.0f; // Initial range within which you can pick up objects
    public float moveForce = 250.0f; // Force applied to move the object
    public float rotationSpeed = 100.0f; // Speed for rotating the object
    public float throwForceMultiplier = 10.0f; // Multiplier for the throw force
    public float rotationThrowForce = 10.0f; // Multiplier for the rotational throw force
    public LayerMask interactableLayer; // Layer for interactable objects

    private Camera _playerCamera;
    private Rigidbody _pickedObject;
    private bool _isDragging;
    private float _rotationX;
    private float _rotationY;

    private Vector3 _lastMousePosition;
    private Vector3 _mouseDelta;
    
    private bool _firstTime;

    private void Start()
    {
        _playerCamera = Camera.main;
    }

    private void OnEnable()
    {
        DragEvents.GetDragging += GetDragging;
    }

    private void OnDisable()
    {
        DragEvents.GetDragging -= GetDragging;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPickObject();
        }

        if (Input.GetMouseButton(0) && _isDragging)
        {
            UpdateRotation();
            //UpdateDistance();
        }

        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            DropObject();
        }

        // Calculate mouse delta
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
        if (_isDragging)
        {
            MoveObject();
        }
    }

    private void TryPickObject()
    {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                _pickedObject = rb;
                _pickedObject.useGravity = false;
                _pickedObject.maxLinearVelocity = 3;
                _isDragging = true;

                // Capture initial rotation
                Vector3 initialRotation = _pickedObject.rotation.eulerAngles;
                _rotationX = initialRotation.y;
                _rotationY = initialRotation.x;

                _lastMousePosition = Input.mousePosition; // Initialize last mouse position
                
                if (!_firstTime) 
                {
                    Narration.DisplayText?.Invoke("What order was it?");
                    _firstTime = true;
                }
            }
        }
    }

    private void DropObject()
    {
        if (_pickedObject != null)
        {
            // Apply throw force
            Vector3 throwForce = _mouseDelta * throwForceMultiplier;
            //pickedObject.linearVelocity = playerCamera.transform.forward * throwForce.z;

            _pickedObject.AddTorque(_playerCamera.transform.right * -_mouseDelta.y * rotationThrowForce);
            _pickedObject.AddTorque(_playerCamera.transform.up * -_mouseDelta.x * rotationThrowForce);

            _pickedObject.useGravity = true;
            _pickedObject.maxLinearVelocity = 0.7f;
            _pickedObject = null;
        }

        _isDragging = false;
    }

    private void MoveObject()
    {
        Vector3 targetPosition = _playerCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2));
        Vector3 forceDirection = targetPosition - _pickedObject.position;

        _pickedObject.linearVelocity = forceDirection * moveForce * Time.fixedDeltaTime;
    }

    private float _rotationZ; // Store cumulative Z-axis rotation
    

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
    }
}
