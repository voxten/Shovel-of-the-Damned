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

    private Coroutine _typingRoutine;
    private Sequence _currentSequence;
    private bool _isDisplaying;

    private void OnEnable() 
    {
        DisplayText += UpdateText;
    }

    private void OnDisable() 
    {
        DisplayText -= UpdateText;
        Cleanup();
    }

    private void Start() 
    {
        fade.DOFade(0, 1).SetEase(Ease.OutExpo).SetDelay(.5f);
    }

    private void Cleanup()
    {
        if (_typingRoutine != null)
        {
            StopCoroutine(_typingRoutine);
            _typingRoutine = null;
        }
        
        if (_currentSequence != null && _currentSequence.IsActive())
        {
            _currentSequence.Kill();
            _currentSequence = null;
        }
    }

    private void UpdateText(string content) 
    {
        Cleanup();
        
        if (label == null) return;
        
        _isDisplaying = true;
        label.text = string.Empty;
        
        // Create a new sequence for the complete animation
        _currentSequence = DOTween.Sequence();
        
        // Fade in
        _currentSequence.Append(displayGroup.DOFade(1, 0.2f).SetEase(Ease.OutExpo));
        
        // Type text
        _currentSequence.AppendCallback(() => {
            _typingRoutine = StartCoroutine(TypewriterEffect(content));
        });
        
        // Wait for typing to complete (handled in coroutine)
    }

    private IEnumerator TypewriterEffect(string content) 
    {
        label.text = string.Empty;
        
        foreach (char letter in content) 
        {
            label.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        yield return new WaitForSeconds(2f);
        
        // Fade out after delay
        _currentSequence = DOTween.Sequence();
        _currentSequence.Append(displayGroup.DOFade(0, 0.2f).SetEase(Ease.OutExpo));
        _currentSequence.OnComplete(() => {
            _isDisplaying = false;
            _currentSequence = null;
        });
        
        _typingRoutine = null;
    }
}