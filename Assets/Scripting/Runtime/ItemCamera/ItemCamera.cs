using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ItemCamera : MonoBehaviour
{
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject cameraMonitor;
    [SerializeField] private GameObject[] handTargets;
    [SerializeField] private Item itemCamera;
    [SerializeField] private TwoBoneIKConstraint handIK;
    [SerializeField] private Animator animator;

    private float handWeight = 0.0f;
    private const float weightStep = 0.02f;
    private const int rotationSteps = 45;
    private const float rotationPerStep = 2.0f;

    private int currentRotationStep = 0;

    private bool _active = false;
    private bool isOpening = false;
    private bool isClosing = false;
    private bool isFullyOpened = false;
    private bool _isPlayerMoving = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && Inventory.InventoryEvents.FindItem(itemCamera))
        {
            ToggleCamera();
        }

        UpdateHandIKWeight();
        HandleOpeningRotation();
        HandleClosingRotation();
    }
    private void CrouchingTargetSet()
    {
        if (animator.GetBool("Crouch"))
        {
            handIK.data.target = handTargets[1].transform;
        }
        else
        {
            handIK.data.target = handTargets[0].transform;
        }
        
    }
    private void UpdateHandIKWeight()
    {
        float targetWeight = _active ? 1.0f : 0.0f;

        // Obni¿ wagê rêki, jeœli gracz siê porusza
        if (_isPlayerMoving && _active)
        {
            targetWeight = 0.2f; // Mo¿na to regulowaæ
        }

        handWeight = Mathf.MoveTowards(handWeight, targetWeight, weightStep);
        handIK.weight = handWeight;

        if (handWeight <= 0.0f && !_active)
        {
            isFullyOpened = false;
            cameraObject.SetActive(false);
        }

        if (handWeight >= 1.0f && !isFullyOpened && !isOpening)
        {
            isOpening = true;
            isFullyOpened = true;
        }
    }

    private void HandleOpeningRotation()
    {
        if (!isOpening) return;

        if (currentRotationStep < rotationSteps)
        {
            currentRotationStep++;
            cameraMonitor.transform.Rotate(0.0f, 0.0f, rotationPerStep, Space.Self);
        }
        else
        {
            isOpening = false;
        }
    }

    private void HandleClosingRotation()
    {
        if (!isClosing) return;

        if (currentRotationStep > 0)
        {
            currentRotationStep--;
            cameraMonitor.transform.Rotate(0.0f, 0.0f, -rotationPerStep, Space.Self);
        }
        else
        {
            isClosing = false;
            _active = false;
        }
    }

    private void ToggleCamera()
    {
        if (!cameraObject.activeSelf)
        {
            cameraObject.SetActive(true);
            _active = true;
        }
        else
        {
            isOpening = false;
            isClosing = true;
        }
    }
    public void SetPlayerMoving(bool isMoving)
    {
        _isPlayerMoving = isMoving;
    }
}
