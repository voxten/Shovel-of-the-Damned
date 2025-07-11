using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button quitButton;
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
        if (PlayerDeathUIPlayerDeathUIManager.DeathEvents.GetIsPlayerDead()) return;
        if (Input.GetKeyDown(KeyCode.Escape) && !NoteUIManager.NoteActions.GetIsOn() && !InventoryUIManager.InventoryUIEvents.GetIsOn() && !InteractionSystem.InteractionEvents.CheckPuzzleInteraction())
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
        AudioListener.pause = false;
        _isOn = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        FirstPersonController.PlayerEvents.ToggleMoveCamera(false);
        Time.timeScale = 0f;
        Utilitis.SetCursorState(false);
        AudioListener.pause = true;
        _isOn = true;
    }

    private void SetPause()
    {
        resumeButton.onClick.AddListener(Resume);
        loadButton.onClick.AddListener(Load);
        menuButton.onClick.AddListener(LoadMainMenu);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void UnsetPause()
    {
        resumeButton.onClick.RemoveAllListeners();
        loadButton.onClick.RemoveAllListeners();
        menuButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.SceneEvents.AnimateLoadScene("MainMenu");
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

    private void Load()
    {
        Resume();
        SavingSystem.SavingSystemEvents.Load();
    }
}