using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ItemCamera : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] private Item itemCamera;
    [SerializeField] private RigBuilder RigBuilder;
    [SerializeField] private Rig handMover;
    private float _handMoverWeightHelp = 0.0f;
    void Start()
    {

    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && Inventory.InventoryEvents.FindItem(itemCamera))
        {
            ChangeCameraEnable();
        }
        if (camera.activeSelf && handMover.weight != 1.0f)
        {
            _handMoverWeightHelp += 0.02f;
            handMover.weight = _handMoverWeightHelp;
        }
        else if(!camera.activeSelf && handMover.weight != 0.0f)
        {
            _handMoverWeightHelp -= 0.02f;
            handMover.weight = _handMoverWeightHelp;
        }
        else if(_handMoverWeightHelp <= 0.0f)
        {
            _handMoverWeightHelp = -0.02f;
            //RigBuilder.enabled = false;
        }
        else if (_handMoverWeightHelp >= 1.0f)
        {
            
            _handMoverWeightHelp = 1.02f;
        }
    }

    private void ChangeCameraEnable()
    {
        //RigBuilder.enabled = true;
        camera.SetActive(!camera.activeSelf);
    }

    
}
