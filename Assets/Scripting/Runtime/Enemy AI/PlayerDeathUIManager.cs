using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDeathUIPlayerDeathUIManager : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button loadLastSaveButton;
    [SerializeField] private Button quitButton;
    private bool _playerIsDead;
    private CanvasGroup _canvasGroup;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        _canvasGroup = deathPanel.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f; // Initialize as fully transparent
    }

    private void OnEnable()
    {
        DeathEvents.KillPlayer += KillPlayer;
        DeathEvents.GetIsPlayerDead += GetIsPlayerDead;
        DeathEvents.SetIsPlayerDead += SetIsPlayerDead;
    }

    private void OnDisable()
    {
        DeathEvents.KillPlayer -= KillPlayer;
        DeathEvents.GetIsPlayerDead -= GetIsPlayerDead;
        DeathEvents.SetIsPlayerDead -= SetIsPlayerDead;
        
        // Stop any running fade coroutine when disabled
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
    }
    
    private void KillPlayer()
    {
        _playerIsDead = true;
        SetButtons();
        deathPanel.SetActive(true);

        Utilitis.SetCursorState(false);
        
        // Start fade coroutine
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float duration = 2f; // 2 seconds
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure alpha is exactly 1 at the end
        _canvasGroup.alpha = 1f;
    }

    private bool GetIsPlayerDead()
    {
        return _playerIsDead;
    }

    private void SetIsPlayerDead()
    {
        _playerIsDead = true;
    }
    
    private void SetButtons()
    {
        backToMenuButton.onClick.AddListener(LoadMainMenu);
        loadLastSaveButton.onClick.AddListener(LoadLastSave);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void UnsetButtons()
    {
        backToMenuButton.onClick.RemoveAllListeners();
        loadLastSaveButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }
    
    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.SceneEvents.AnimateLoadScene("MainMenu");
    }

    private void LoadLastSave()
    {
        Time.timeScale = 1f;
        SavingSystem.SavingSystemEvents.Load();
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    public static class DeathEvents 
    {
        public static Action KillPlayer;
        public static Func<bool> GetIsPlayerDead;
        public static Action SetIsPlayerDead;
    }
}