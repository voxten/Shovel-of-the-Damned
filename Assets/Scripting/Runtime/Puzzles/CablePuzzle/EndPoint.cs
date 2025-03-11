using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class GameObjectEvent : UnityEvent<GameObject> { }
public class EndPoint : MonoBehaviour
{
    private bool haveCable;
    private Collider _collider;
    private HingeJoint _hingeJoint;
    private GameObject cable;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _hingeJoint = GetComponent<HingeJoint>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0) && haveCable)
        {
            _hingeJoint.connectedBody = cable.GetComponent<Rigidbody>();
        }

        if (Input.GetMouseButtonDown(0))
        {
            _hingeJoint.connectedBody = null;
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Last")
        {
            haveCable = true;
            cable = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Last")
        {
            haveCable = false;
        }
    }
    
}
