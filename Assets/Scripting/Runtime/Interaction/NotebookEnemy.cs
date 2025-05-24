using System;
using UnityEngine;

public class NotebookEnemy : MonoBehaviour
{
    [SerializeField] private Item itemToCheck;
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private TutorialObject tutorialObject;

    private bool _wasActivated;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_wasActivated)
        {
            var item = Inventory.InventoryEvents.FindItem(itemToCheck);
            if (item)
            {
                enemyAI.gameObject.SetActive(true);
                TutorialManager.TutorialManagerEvents.startTutorial(tutorialObject);
                _wasActivated = true;
            }
        }
    }
}
