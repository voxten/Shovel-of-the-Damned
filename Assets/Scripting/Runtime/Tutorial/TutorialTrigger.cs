using System;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private TutorialObject tutorialObject;
    private bool _isTriggering;
    private bool _tutorialCompleted;

    private void Update()
    {
        if (!_isTriggering) return;
        if (Input.GetKeyDown(tutorialObject.keyCode))
        {
            _tutorialCompleted = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggering = true;
            if (_tutorialCompleted) return;
            TutorialManager.TutorialManagerEvents.startTutorial(tutorialObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggering = false;
        }
    }
}
