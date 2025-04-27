using System;
using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    private CanvasGroup _tutorialCanvasGroup;

    private Coroutine _currentTutorialRoutine;

    private void Start()
    {
        _tutorialCanvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
        _tutorialCanvasGroup.alpha = 0f;
        tutorialPanel.SetActive(false);
    }

    private void OnEnable()
    {
        TutorialManagerEvents.startTutorial += StartTutorial;
        TutorialManagerEvents.stopTutorial += StopTutorial;
    }

    private void OnDisable()
    {
        TutorialManagerEvents.startTutorial -= StartTutorial;
        TutorialManagerEvents.stopTutorial -= StopTutorial;
    }

    private void StartTutorial(TutorialObject tutorialObject)
    {
        StopTutorial(); // Stop any running tutorial before starting a new one
        _currentTutorialRoutine = StartCoroutine(AnimateTutorial(tutorialObject));
    }

    private IEnumerator AnimateTutorial(TutorialObject tutorialObject)
    {
        tutorialPanel.SetActive(true);
        tutorialText.text = "";

        _tutorialCanvasGroup.alpha = 0f;
        _tutorialCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutCubic);

        yield return StartCoroutine(TypeText(tutorialObject.tutorialDescription));

        yield return new WaitForSeconds(2.5f);

        _tutorialCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(0.5f);

        tutorialPanel.SetActive(false);
        _currentTutorialRoutine = null;
    }

    private IEnumerator TypeText(string message)
    {
        tutorialText.text = "";
        foreach (var c in message)
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(0.025f); // typing speed
        }
    }

    private void StopTutorial()
    {
        if (_currentTutorialRoutine != null)
        {
            StopCoroutine(_currentTutorialRoutine);
            _currentTutorialRoutine = null;
        }
        
        _tutorialCanvasGroup.DOKill();
        _tutorialCanvasGroup.alpha = 0f;
        tutorialPanel.SetActive(false);
    }

    public static class TutorialManagerEvents
    {
        public static Action<TutorialObject> startTutorial;
        public static Action stopTutorial;
    }
}
