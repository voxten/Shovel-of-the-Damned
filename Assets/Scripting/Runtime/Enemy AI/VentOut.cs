using UnityEngine;

public class VentOut : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private Vent vent;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("VentEnemy"))
        {
            Debug.Log("Entered VentOut");
            enemyAI.SetVentOutState(vent);
        }
    }
}
