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

    private void Start()
    {
        _tutorialCanvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
        _tutorialCanvasGroup.alpha = 0f;
        tutorialPanel.SetActive(false);
    }

    private void OnEnable()
    {
        TutorialManagerEvents.startTutorial += StartTutorial;
    }

    private void OnDisable()
    {
        TutorialManagerEvents.startTutorial -= StartTutorial;
    }

    private void StartTutorial(TutorialObject tutorialObject)
    {
        StartCoroutine(AnimateTutorial(tutorialObject));
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
    }

    private IEnumerator TypeText(string message)
    {
        tutorialText.text = "";
        foreach (var c in message)
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(0.025f); // speed of typing
        }
    }

    public static class TutorialManagerEvents
    {
        public static Action<TutorialObject> startTutorial;
    }
}
