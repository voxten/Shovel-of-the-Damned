using System;
using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TutorialObject tutorialRotateObject;
    private CanvasGroup _tutorialCanvasGroup;

    private Coroutine _currentTutorialRoutine;
    private Sequence _currentSequence;

    private void Start()
    {
        _tutorialCanvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
        _tutorialCanvasGroup.alpha = 0f;
        tutorialPanel.SetActive(false);
    }

    private void OnEnable()
    {
        TutorialManagerEvents.startTutorial += StartTutorial;
        TutorialManagerEvents.startRotateTutorial += StartRotateTutorial;
        TutorialManagerEvents.stopTutorial += StopTutorial;
    }

    private void OnDisable()
    {
        TutorialManagerEvents.startTutorial -= StartTutorial;
        TutorialManagerEvents.startRotateTutorial -= StartRotateTutorial;
        TutorialManagerEvents.stopTutorial -= StopTutorial;
        Cleanup();
    }

    private void Cleanup()
    {
        // Stop any running coroutine
        if (_currentTutorialRoutine != null)
        {
            StopCoroutine(_currentTutorialRoutine);
            _currentTutorialRoutine = null;
        }
        
        // Kill any active tween sequence
        if (_currentSequence != null && _currentSequence.IsActive())
        {
            _currentSequence.Kill();
            _currentSequence = null;
        }
    }

    private void StartTutorial(TutorialObject tutorialObject)
    {
        // Clean up any existing tutorial first
        Cleanup();
        
        // Start new tutorial
        _currentTutorialRoutine = StartCoroutine(AnimateTutorial(tutorialObject));
    }
    
    private void StartRotateTutorial()
    {
        // Clean up any existing tutorial first
        Cleanup();
        
        // Start new tutorial
        _currentTutorialRoutine = StartCoroutine(AnimateTutorial(tutorialRotateObject));
    }

    private IEnumerator AnimateTutorial(TutorialObject tutorialObject)
    {
        // Initialize panel
        tutorialPanel.SetActive(true);
        tutorialText.text = "";
        _tutorialCanvasGroup.alpha = 0f;

        // Fade in
        _currentSequence = DOTween.Sequence();
        _currentSequence.Append(_tutorialCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutCubic));
        yield return _currentSequence.WaitForCompletion();

        // Type out the text
        yield return TypeText(tutorialObject.tutorialDescription);

        // Wait for display duration
        yield return new WaitForSeconds(2.5f);

        // Fade out
        _currentSequence = DOTween.Sequence();
        _currentSequence.Append(_tutorialCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InCubic));
        yield return _currentSequence.WaitForCompletion();

        // Clean up
        tutorialPanel.SetActive(false);
        _currentTutorialRoutine = null;
        _currentSequence = null;
    }

    private IEnumerator TypeText(string message)
    {
        tutorialText.text = "";
        foreach (var c in message)
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(0.025f);
        }
    }

    private void StopTutorial()
    {
        Cleanup();
        _tutorialCanvasGroup.alpha = 0f;
        tutorialPanel.SetActive(false);
    }

    public static class TutorialManagerEvents
    {
        public static Action<TutorialObject> startTutorial;
        public static Action stopTutorial;
        public static Action startRotateTutorial;
    }
}