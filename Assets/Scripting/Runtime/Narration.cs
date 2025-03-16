using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using System.Collections;

public class Narration : MonoBehaviour 
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private CanvasGroup displayGroup;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private CanvasGroup fade;
    public static Action<string> DisplayText;

    private void OnEnable() 
    {
        DisplayText += UpdateText;
    }

    private void OnDisable() 
    {
        DisplayText -= UpdateText;
    }

    private void Start() 
    {
        Invoke(nameof(StartDialog), 2);
        fade.DOFade(0, 1).SetEase(Ease.OutExpo).SetDelay(.5f);
    }

    private void StartDialog() 
    {
        DisplayText?.Invoke("I have to make it in time...");
    }

    private void UpdateText(string _content) 
    {
        StopAllCoroutines();
        if (label != null) {
            label.text = string.Empty;
            displayGroup.DOFade(1, .2f).SetEase(Ease.OutExpo).OnComplete(() => {
                StartCoroutine(TypewriterEffect(_content));
            });
        }
    }

    private IEnumerator TypewriterEffect(string content) 
    {
        if (label != null) 
        {
            label.text = string.Empty;
            foreach (char letter in content) 
            {
                label.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(2f);
            displayGroup.DOFade(0, .2f).SetEase(Ease.OutExpo);
        }
    }
}