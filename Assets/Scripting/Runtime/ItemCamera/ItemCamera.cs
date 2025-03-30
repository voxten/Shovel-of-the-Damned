using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ItemCamera : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject cameraMonitor;
    [SerializeField] private Item itemCamera;
    [SerializeField] private TwoBoneIKConstraint handMover;
    private float _handMoverWeightHelp = 0.0f;
    private int _r = 0;
    private bool _active = false;
    private bool _toOpen = false;
    private bool _toClose = false;
    private bool _isOpened = false;
    void Start()
    {

    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && Inventory.InventoryEvents.FindItem(itemCamera))
        {
            ChangeCameraEnable();
        }
        if (_active && handMover.weight != 1.0f)
        {
            _handMoverWeightHelp += 0.02f;
            handMover.weight = _handMoverWeightHelp;
        }
        else if(!_active && handMover.weight != 0.0f)
        {
            _handMoverWeightHelp -= 0.02f;
            handMover.weight = _handMoverWeightHelp;
        }
        else if(_handMoverWeightHelp <= 0.0f)
        {
            _handMoverWeightHelp = -0.02f;
            _isOpened = false;
            camera.SetActive(false);
        }
        else if (_handMoverWeightHelp >= 1.0f && !_isOpened)
        {
            _toOpen = true;
            _handMoverWeightHelp = 1.02f;
        }
        if(_toOpen)
        {
            if (_r < 45)
            {
                _r += 1;
                cameraMonitor.transform.Rotate(0.0f, 0.0f, 2.0f, Space.Self);
            }
            else _isOpened = true;
        }
        else if(_toClose)
        {
            if (_r > 0)
            {
                _r -= 1;
                cameraMonitor.transform.Rotate(0.0f, 0.0f, -2.0f, Space.Self);
            }
            else
            {
                _toClose = false;
                _active = false;
            }
        }
    }

    private void ChangeCameraEnable()
    {
        if (!camera.activeSelf)
        {
            camera.SetActive(true);
            _active = true;
        }
        else
        {
            _toOpen = false;
            _toClose = true;
        }
    }
}
