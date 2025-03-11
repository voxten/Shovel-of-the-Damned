using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _playButton, _optionsButton, _creditsButton, _quitButton;
    [SerializeField] private CanvasGroup _mainMenuFade;
    [Space]
    
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private CanvasGroup _optionsPanelFade;
    [SerializeField] private Button _optionsBackButton;
    
    [Space]
    
    [SerializeField] private GameObject _creditsPanel;
    [SerializeField] private CanvasGroup _creditsPanelFade;
    [SerializeField] private Button _creditsBackButton;
    
    [Space]
    
    [SerializeField] private string _mainGameSceneName = "Main Scene";

    
    [Space]
    
    [SerializeField] private Slider _masterVolumeSlider, _musicVolumeSlider, _sfxVolumeSlider;
    [SerializeField] private AudioMixer _mainMixer;

    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private CanvasGroup gameInFade;
    
    private void OnEnable()
    {
        _playButton.onClick.AddListener(Play);
        _optionsButton.onClick.AddListener(ShowOptions);
         _creditsButton.onClick.AddListener(ShowCredits);
        _quitButton.onClick.AddListener(Quit);
        
        _optionsBackButton.onClick.AddListener(HideOptions);
        _creditsBackButton.onClick.AddListener(HideCredits);
        
        _masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }
    
    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(Play);
        _optionsButton.onClick.RemoveListener(ShowOptions);
        _creditsButton.onClick.RemoveListener(ShowCredits);
        _quitButton.onClick.RemoveListener(Quit);
        
        _optionsBackButton.onClick.RemoveListener(HideOptions);
        _creditsBackButton.onClick.RemoveListener(HideCredits);
        
        _masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        _musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }

    private void Start() {
        gameInFade.DOFade(0, .6f).SetEase(Ease.OutExpo).SetDelay(1).OnComplete(() => gameInFade.gameObject.SetActive(false));
        Cursor.lockState = CursorLockMode.None;
    }

    private void Play() {
        fadeCanvas.DOFade(1, .5f).SetEase(Ease.OutExpo).OnComplete(() => SceneManager.LoadScene(_mainGameSceneName));
    }

    private void ShowOptions()
    {
        _optionsPanel.SetActive(true);
        _optionsPanelFade.DOFade(1, .5f).SetEase(Ease.OutExpo);
        _mainMenuFade.DOFade(0, .5f).SetEase(Ease.OutExpo);
    }

    private void ShowCredits()
    {
        _creditsPanel.SetActive(true);
        _creditsPanelFade.DOFade(1, .5f).SetEase(Ease.OutExpo);
        _mainMenuFade.DOFade(0, .5f).SetEase(Ease.OutExpo);
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void HideOptions()
    {
        _optionsPanelFade.DOFade(0, .5f).SetEase(Ease.OutExpo).OnComplete(()=>_optionsPanel.SetActive(false));
        _mainMenuFade.DOFade(1, .5f).SetEase(Ease.OutExpo);
    }
    
    private void HideCredits()
    {
        _creditsPanelFade.DOFade(0, .5f).SetEase(Ease.OutExpo).OnComplete(()=>_creditsPanel.SetActive(false));
        _mainMenuFade.DOFade(1, .5f).SetEase(Ease.OutExpo);
    }
    
    
    private void SetMasterVolume(float value)
    {
        float volume = Mathf.Log10(value) * 20;
        _mainMixer.SetFloat("MasterVolume", volume);
    }
    
    private void SetMusicVolume(float value)
    {
        float volume = Mathf.Log10(value) * 20;
        _mainMixer.SetFloat("MusicVolume", volume);
    }
    
    private void SetSFXVolume(float value)
    {
        float volume = Mathf.Log10(value) * 20;
        _mainMixer.SetFloat("SFXVolume", volume);
    }
}
