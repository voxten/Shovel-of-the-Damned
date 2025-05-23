using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    private bool _isTriggered;
    private Inventory _FPC;
    private float _lastY;

    private void Start()
    {
        _FPC = FindAnyObjectByType<Inventory>();
        _lastY = transform.position.y;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggered = true;

            float currentY = transform.position.y;
            float deltaY = currentY - _lastY;

            // Przesuwamy gracza o tyle, o ile przemieœci³a siê platforma w osi Y
            Vector3 playerPos = _FPC.transform.position;
            _FPC.transform.position = new Vector3(playerPos.x, playerPos.y + deltaY, playerPos.z);

            _lastY = currentY;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggered = false;
        }
    }

    public bool GetIsTriggered()
    {
        return _isTriggered;
    }
}
