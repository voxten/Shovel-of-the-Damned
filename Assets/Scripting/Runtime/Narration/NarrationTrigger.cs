using UnityEngine;

public class NarrationTrigger : MonoBehaviour 
{
    [SerializeField] private string message;
    [SerializeField] private bool oneTime;
    private bool _triggered;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            if(_triggered) return;
            Narration.DisplayText?.Invoke(message);
            _triggered = true;
        }
    }
}
