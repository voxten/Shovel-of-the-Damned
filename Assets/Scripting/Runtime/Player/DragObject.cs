using UnityEngine;

public class DragObject : MonoBehaviour
{
    public float pickupRange = 5.0f; // Initial range within which you can pick up objects
    public float moveForce = 250.0f; // Force applied to move the object
    public float rotationSpeed = 100.0f; // Speed for rotating the object
    public float throwForceMultiplier = 10.0f; // Multiplier for the throw force
    public float rotationThrowForce = 10.0f; // Multiplier for the rotational throw force
    public float distanceChangeSpeed = 2.0f; // Speed at which the distance changes
    public LayerMask interactableLayer; // Layer for interactable objects

    private Camera playerCamera;
    private Rigidbody pickedObject;
    private bool isDragging = false;
    private float rotationX;
    private float rotationY;
    private float currentDistance;

    private Vector3 lastMousePosition;
    private Vector3 mouseDelta;

    void Start()
    {
        playerCamera = Camera.main;
        currentDistance = pickupRange; // Set initial distance
    }

    private bool firstTime;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPickObject();
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            //UpdateRotation();
            //UpdateDistance();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            DropObject();
        }

        // Calculate mouse delta
        if (isDragging)
        {
            mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;
        }
        else
        {
            lastMousePosition = Input.mousePosition;
            mouseDelta = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            MoveObject();
        }
    }

    void TryPickObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactableLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                pickedObject = rb;
                pickedObject.useGravity = false;
                pickedObject.maxLinearVelocity = 3;
                isDragging = true;

                // Capture initial rotation
                Vector3 initialRotation = pickedObject.rotation.eulerAngles;
                rotationX = initialRotation.y;
                rotationY = initialRotation.x;

                lastMousePosition = Input.mousePosition; // Initialize last mouse position
                
                if (!firstTime) {
                    Narration.DisplayText?.Invoke("What order was it?");
                    firstTime = true;
                }
            }
        }
    }

    void DropObject()
    {
        if (pickedObject != null)
        {
            // Apply throw force
            Vector3 throwForce = mouseDelta * throwForceMultiplier;
            //pickedObject.linearVelocity = playerCamera.transform.forward * throwForce.z;

            pickedObject.AddTorque(playerCamera.transform.right * -mouseDelta.y * rotationThrowForce);
            pickedObject.AddTorque(playerCamera.transform.up * -mouseDelta.x * rotationThrowForce);

            pickedObject.useGravity = true;
            pickedObject.maxLinearVelocity = 0.7f;
            pickedObject = null;
        }

        isDragging = false;
    }

    void MoveObject()
    {
        Vector3 targetPosition = playerCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2));
        Vector3 forceDirection = targetPosition - pickedObject.position;

        pickedObject.linearVelocity = forceDirection * moveForce * Time.fixedDeltaTime;
    }

    void UpdateRotation()
    {
        rotationX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(rotationY, rotationX, 0);
        pickedObject.MoveRotation(targetRotation);
    }

    void UpdateDistance()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentDistance += distanceChangeSpeed * Time.deltaTime;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentDistance -= distanceChangeSpeed * Time.deltaTime;
        }

        currentDistance = Mathf.Clamp(currentDistance, 1.0f, pickupRange);
    }
}
