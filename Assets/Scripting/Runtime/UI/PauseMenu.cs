using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuPanel;
    
    [Space] 
    
    [SerializeField] private Button _continueButton, _quitButton;
    
    public static PauseMenu Instance { get; private set; }
    public bool IsPaused => _pauseMenuPanel.activeSelf;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnEnable()
    {
        _continueButton.onClick.AddListener(Continue);
        _quitButton.onClick.AddListener(Quit);
    }
    
    private void OnDisable()
    {
        _continueButton.onClick.RemoveListener(Continue);
        _quitButton.onClick.RemoveListener(Quit);
    }

    private void TogglePause()
    {
        _pauseMenuPanel.SetActive(!_pauseMenuPanel.activeSelf);
        FirstPersonController.PlayerEvents.ToggleMove();
        Time.timeScale = _pauseMenuPanel.activeSelf ? 0 : 1;
        Cursor.lockState = _pauseMenuPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }
    
    private void SetPauseState(bool state)
    {
        _pauseMenuPanel.SetActive(state);
        FirstPersonController.PlayerEvents.ToggleMove();
        Time.timeScale = state ? 0 : 1;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void Continue()
    {
        SetPauseState(false);
    }

    private void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
