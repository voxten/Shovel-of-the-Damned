using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !enemyAI.GetIsInVent())
        {
            enemyAI.SetAttackState();
        }
    }
}
