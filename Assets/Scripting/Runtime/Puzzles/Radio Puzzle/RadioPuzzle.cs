using System;
using System.Collections;
using UnityEngine;

public class RadioPuzzle : PuzzleObject
{
    [SerializeField] private RadioScroll[] radioScrolls;
    [SerializeField] private float noiseVolume = 0.2f;
    [SerializeField] private float codeVolume = 0.8f;
    [SerializeField] private float codeRepeatDelay = 3f;
    [SerializeField] private AudioLibraryData audioLibrary;
    [SerializeField] private Item finishItem;
    
    private Coroutine _radioLoopCoroutine;
    private Coroutine _codePlayCoroutine;
    private Sound _currentSecondarySound = Sound.None;
    private bool _isCodePlaying;

    private void Awake()
    {
        SoundManager.PlaySound3D(Sound.RadioNoiseLoop, transform, noiseVolume);
        _radioLoopCoroutine = StartCoroutine(LoopRadio());
    }

    private float GetAudioClipLength(Sound type)
    {
        var clip = audioLibrary.GetAudioClip(type);
        return clip != null ? clip.length : 0f;
    }

    private void OnEnable()
    {
        RadioEvents.CheckNeedles += CheckNeedles;
        RadioEvents.GetIsFinished += GetIsFinished;
    }
    
    private void OnDisable()
    {
        RadioEvents.CheckNeedles -= CheckNeedles;
        RadioEvents.GetIsFinished -= GetIsFinished;
        
        if (_radioLoopCoroutine != null) StopCoroutine(_radioLoopCoroutine);
        if (_codePlayCoroutine != null) StopCoroutine(_codePlayCoroutine);
        
        SoundManager.StopSound3D(Sound.RadioNoiseLoop, transform);
        SoundManager.StopSound3D(Sound.RadioDistCode, transform);
        SoundManager.StopSound3D(Sound.RadioCode, transform);
        _isCodePlaying = false;
    }

    public override void OpenPuzzle()
    {

    }

    public override void QuitPuzzle()
    {

    }

    protected override void EndPuzzle()
    {
        base.EndPuzzle();
        Inventory.InventoryEvents.AddItem(finishItem);
    }
    
    private bool GetIsFinished()
    {
        return isFinished;
    }

    private void CheckNeedles()
    {
        var count = 0;
        foreach (var scroll in radioScrolls)
        {
            if (scroll.CheckNeedlePosition())
            {
                count++;
            }
        }
        
        Debug.Log(count + " radio scrolls are finished");

        if (count == 2)
        {
            EndPuzzle();
        }
    }

    private IEnumerator LoopRadio()
    {
        while (true)
        {
            var count = GetCorrectNeedleCount();

            Sound soundToPlay = count == 1 ? Sound.RadioDistCode : 
                count >= 2 ? Sound.RadioCode : Sound.None;

            if (soundToPlay != _currentSecondarySound)
            {
                HandleSoundTransition(soundToPlay);
            }
            
            yield return new WaitForSeconds(0.3f);
        }
    }

    private int GetCorrectNeedleCount()
    {
        int count = 0;
        foreach (var scroll in radioScrolls)
        {
            if (scroll.CheckNeedlePosition()) count++;
        }
        return count;
    }
    
    private void HandleSoundTransition(Sound newSound)
    {
        // Stop previous sound
        if (_currentSecondarySound != Sound.None)
        {
            SoundManager.StopSound3D(_currentSecondarySound, transform);
            
            if (_currentSecondarySound == Sound.RadioCode && _codePlayCoroutine != null)
            {
                StopCoroutine(_codePlayCoroutine);
                _codePlayCoroutine = null;
                _isCodePlaying = false;
            }
        }
        
        // Play new sound
        if (newSound != Sound.None)
        {
            if (newSound == Sound.RadioCode)
            {
                _codePlayCoroutine = StartCoroutine(PlayCodeRepeating());
            }
            else
            {
                SoundManager.PlaySound3D(newSound, transform, codeVolume);
            }
        }
        
        _currentSecondarySound = newSound;
    }

    private IEnumerator PlayCodeRepeating()
    {
        while (true)
        {
            if (!_isCodePlaying)
            {
                _isCodePlaying = true;
                SoundManager.PlaySound3D(Sound.RadioCode, transform, codeVolume);
                
                // Get clip length
                float clipLength = GetAudioClipLength(Sound.RadioCode);
                yield return new WaitForSeconds(clipLength);
                
                _isCodePlaying = false;
                yield return new WaitForSeconds(codeRepeatDelay);
            }
            else
            {
                yield return null;
            }
        }
    }
    
    public static class RadioEvents
    {
        public static Action CheckNeedles;
        public static Func<bool> GetIsFinished;
    }
}