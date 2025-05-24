using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !enemyAI.GetIsInVent())
        {
            FindFirstObjectByType<FlashlightOptions>(FindObjectsInactive.Include).gameObject.SetActive(false);
            FindFirstObjectByType<UsingFlashLight>(FindObjectsInactive.Include).enabled = false;
            enemyAI.SetAttackState();
        }
    }
}
