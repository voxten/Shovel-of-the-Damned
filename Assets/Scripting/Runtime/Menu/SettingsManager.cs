using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider, musicSlider, sfxSlider;
    [SerializeField] private TMP_Text masterLabel, musicLabel, sfxLabel;

    [Header("Display Mode Settings")]
    [SerializeField] private TMP_Text displayModeText;
    [SerializeField] private Button prevDisplayModeButton, nextDisplayModeButton;
    
    [Header("Resolution Settings")]
    [SerializeField] private TMP_Text resolutionLabel;
    [SerializeField] private Button prevResolutionButton, nextResolutionButton;
    
    [Header("Refresh Rate Settings")]
    [SerializeField] private TMP_Text refreshRateLabel;
    [SerializeField] private Button prevRefreshRateButton, nextRefreshRateButton;
    
    [Header("Quality Settings")]
    [SerializeField] private TMP_Text qualityLabel;
    [SerializeField] private Button prevQualityButton, nextQualityButton;
    
    [Header("Apply Settings")]
    [SerializeField] private Button applyResolutionButton;
    
    private enum ScreenMode { Fullscreen, Windowed, Borderless }
    private ScreenMode _currentMode;

    private Resolution[] uniqueResolutions;
    private uint[] uniqueRefreshRates;
    private string[] qualityLevels;

    private int _currentResolutionIndex;
    private int _currentRefreshRateIndex;
    private int _currentQualityIndex;

    private void Awake()
    {
        LoadSettings();
        PopulateResolutionAndRefreshRate();
        PopulateQualityLevels();
        AssignUIEvents();
        LoadResolutionAndRefreshRate();
        LoadQualityLevel();
    }

    private void AssignUIEvents()
    {
        masterSlider.onValueChanged.AddListener(_ => SetMasterVolume());
        musicSlider.onValueChanged.AddListener(_ => SetMusicVolume());
        sfxSlider.onValueChanged.AddListener(_ => SetSfxVolume());

        prevResolutionButton.onClick.AddListener(PreviousResolution);
        nextResolutionButton.onClick.AddListener(NextResolution);
        
        prevDisplayModeButton.onClick.AddListener(PreviousDisplayMode);
        nextDisplayModeButton.onClick.AddListener(NextDisplayMode);
        
        prevRefreshRateButton.onClick.AddListener(PreviousRefreshRate);
        nextRefreshRateButton.onClick.AddListener(NextRefreshRate);
        
        prevQualityButton.onClick.AddListener(PreviousQuality);
        nextQualityButton.onClick.AddListener(NextQuality);
        
        applyResolutionButton.onClick.AddListener(ApplySettings);
    }

    private void PopulateQualityLevels()
    {
        qualityLevels = QualitySettings.names;
        _currentQualityIndex = QualitySettings.GetQualityLevel();
        UpdateQualityLabel();
    }

    private void NextQuality()
    {
        if (_currentQualityIndex < qualityLevels.Length - 1)
        {
            _currentQualityIndex++;
        }
        UpdateQualityLabel();
    }

    private void PreviousQuality()
    {
        if (_currentQualityIndex > 0)
        {
            _currentQualityIndex--;
        }
        UpdateQualityLabel();
    }

    private void UpdateQualityLabel()
    {
        qualityLabel.text = qualityLevels[_currentQualityIndex];
    }

    private void PopulateResolutionAndRefreshRate()
    {
        var allResolutions = Screen.resolutions;
        
        var resolutionSet = new HashSet<string>();
        var refreshRateSet = new HashSet<uint>();
        
        var tempResolutions = new List<Resolution>();
        var tempRefreshRates = new List<uint>();
        
        foreach (var res in allResolutions)
        {
            var resKey = res.width + "x" + res.height;
            if (resolutionSet.Add(resKey))
            {
                tempResolutions.Add(new Resolution { width = res.width, height = res.height });
            }

            var refreshRate = (uint)Mathf.RoundToInt((float)res.refreshRateRatio.numerator / res.refreshRateRatio.denominator);
            if (refreshRateSet.Add(refreshRate))
            {
                tempRefreshRates.Add(refreshRate);
            }
        }
        
        uniqueResolutions = tempResolutions.ToArray();
        uniqueRefreshRates = tempRefreshRates.OrderBy(r => r).ToArray();
        
        if (uniqueResolutions.Length == 0)
        {
            Debug.LogError("No unique resolutions found! Adding fallback 1920x1080.");
            uniqueResolutions = new[] { new Resolution { width = 1920, height = 1080 } };
        }

        if (uniqueRefreshRates.Length == 0)
        {
            Debug.LogWarning("No refresh rates found! Adding fallback 60Hz.");
            uniqueRefreshRates = new uint[] { 60 };
        }
        
        _currentResolutionIndex = Array.FindIndex(uniqueResolutions, r => r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height);
        if (_currentResolutionIndex == -1) _currentResolutionIndex = 0;

        var currentRefreshRate = Screen.currentResolution.refreshRateRatio.numerator / Screen.currentResolution.refreshRateRatio.denominator;
        _currentRefreshRateIndex = Array.FindIndex(uniqueRefreshRates, r => r == currentRefreshRate);
        if (_currentRefreshRateIndex == -1) _currentRefreshRateIndex = 0;

        UpdateResolutionLabel();
        UpdateRefreshRateLabel();
    }
    
    private void NextResolution()
    {
        if (_currentResolutionIndex < uniqueResolutions.Length - 1)
        {
            _currentResolutionIndex++;
        }
        UpdateResolutionLabel();
    }

    private void PreviousResolution()
    {
        if (_currentResolutionIndex > 0)
        {
            _currentResolutionIndex--;
        }
        UpdateResolutionLabel();
    }

    private void NextRefreshRate()
    {
        if (_currentRefreshRateIndex < uniqueRefreshRates.Length - 1)
        {
            _currentRefreshRateIndex++;
        }
        UpdateRefreshRateLabel();
    }

    private void PreviousRefreshRate()
    {
        if (_currentRefreshRateIndex > 0)
        {
            _currentRefreshRateIndex--;
        }
        UpdateRefreshRateLabel();
    }

    private void PreviousDisplayMode()
    {
        _currentMode = (ScreenMode)(((int)_currentMode + 2) % 3);
        UpdateDisplayMode();
    }

    private void NextDisplayMode()
    {
        _currentMode = (ScreenMode)(((int)_currentMode + 1) % 3);
        UpdateDisplayMode();
    }
    
    private void UpdateDisplayMode()
    {
        displayModeText.text = _currentMode.ToString();
    }

    private void ApplySettings()
    {
        var selectedResolution = uniqueResolutions[_currentResolutionIndex];
        var selectedRefreshRate = uniqueRefreshRates[_currentRefreshRateIndex];

        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen, (int)selectedRefreshRate);
        QualitySettings.SetQualityLevel(_currentQualityIndex, true);

        PlayerPrefs.SetInt("ResolutionIndex", _currentResolutionIndex);
        PlayerPrefs.SetInt("RefreshRateIndex", _currentRefreshRateIndex);
        PlayerPrefs.SetInt("QualityLevel", _currentQualityIndex);
        ApplyDisplayMode();

        PlayerPrefs.Save();
    }

    private void ApplyDisplayMode()
    {
        switch (_currentMode)
        {
            case ScreenMode.Fullscreen:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case ScreenMode.Windowed:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case ScreenMode.Borderless:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
        UpdateDisplayMode();
        PlayerPrefs.SetInt("ScreenMode", (int)_currentMode);
    }

    private void UpdateResolutionLabel()
    {
        resolutionLabel.text = uniqueResolutions[_currentResolutionIndex].width + " x " + uniqueResolutions[_currentResolutionIndex].height;
    }

    private void UpdateRefreshRateLabel()
    {
        refreshRateLabel.text = uniqueRefreshRates[_currentRefreshRateIndex] + " Hz";
    }

    private void SetMasterVolume()
    {
        masterLabel.text = Mathf.RoundToInt(masterSlider.value + 80).ToString();
        audioMixer.SetFloat("Master", masterSlider.value);
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    }

    private void SetMusicVolume()
    {
        musicLabel.text = Mathf.RoundToInt(musicSlider.value + 80).ToString();
        audioMixer.SetFloat("Music", musicSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    private void SetSfxVolume()
    {
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value + 80).ToString();
        audioMixer.SetFloat("SFX", sfxSlider.value);
        PlayerPrefs.SetFloat("SfxVolume", sfxSlider.value);
    }
    
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");

        if (PlayerPrefs.HasKey("MusicVolume"))
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        
        if (PlayerPrefs.HasKey("SfxVolume"))
            sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
        
        _currentMode = (ScreenMode)PlayerPrefs.GetInt("ScreenMode", (int)ScreenMode.Fullscreen);
        ApplyDisplayMode();
        
        SetMasterVolume();
        SetMusicVolume();
        SetSfxVolume();
    }

    private void LoadResolutionAndRefreshRate()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
            _currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");

        if (PlayerPrefs.HasKey("RefreshRateIndex"))
            _currentRefreshRateIndex = PlayerPrefs.GetInt("RefreshRateIndex");
        
        UpdateResolutionLabel();
        UpdateRefreshRateLabel();
    }

    private void LoadQualityLevel()
    {
        if (PlayerPrefs.HasKey("QualityLevel"))
            _currentQualityIndex = PlayerPrefs.GetInt("QualityLevel");
        
        UpdateQualityLabel();
    }
}