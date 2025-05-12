using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private AudioListener audioListener;
    private bool _isOn;
    
    private void OnEnable()
    {
        PauseEvents.GetIsOn += GetIsOn;
    }

    private void OnDisable()
    {
        PauseEvents.GetIsOn -= GetIsOn;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !NoteUIManager.NoteActions.GetIsOn() && !InventoryUIManager.InventoryUIEvents.GetIsOn())
        {
            if (_isOn)
            {
                Resume();
                UnsetPause();
            }
            else
            {
                Pause();
                SetPause();
            }
        }
    }

    private void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        FirstPersonController.PlayerEvents.ToggleMoveCamera(true);
        Utilitis.SetCursorState(true);
        audioListener.enabled = true;
        _isOn = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        FirstPersonController.PlayerEvents.ToggleMoveCamera(false);
        Time.timeScale = 0f;
        Utilitis.SetCursorState(false);
        audioListener.enabled = false;
        _isOn = true;
    }

    private void SetPause()
    {
        resumeButton.onClick.AddListener(Resume);
        menuButton.onClick.AddListener(LoadMainMenu);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void UnsetPause()
    {
        resumeButton.onClick.RemoveAllListeners();
        menuButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
    
    private bool GetIsOn()
    {
        return _isOn;
    }
    
    public static class PauseEvents
    {
        public static Func<bool> GetIsOn;
    }
}