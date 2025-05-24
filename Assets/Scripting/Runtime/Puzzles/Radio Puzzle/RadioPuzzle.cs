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
    private Outline _currentOutline;
    
    [SerializeField] private TutorialObject tutorialObject;

    private void Awake()
    {
        SoundManager.PlaySound3D(Sound.RadioNoiseLoop, transform, null, noiseVolume);
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
    }

    protected override void EndPuzzle()
    {
        base.EndPuzzle();
        if (!Inventory.InventoryEvents.FindItem(finishItem))
        {
            Inventory.InventoryEvents.AddItem(finishItem);
        } 
    }
    
    private bool GetIsFinished()
    {
        return isFinished;
    }

    public override void OpenPuzzle()
    {
        base.OpenPuzzle();
        TutorialManager.TutorialManagerEvents.startTutorial(tutorialObject);
        foreach (var scroll in radioScrolls)
        {
            _currentOutline = scroll.gameObject.GetComponent<Outline>();
            if (_currentOutline == null)
            {
                _currentOutline = scroll.gameObject.AddComponent<Outline>();
            }
            _currentOutline.OutlineMode = Outline.Mode.OutlineVisible;
            _currentOutline.OutlineColor = Color.green;
            _currentOutline.OutlineWidth = 2f;
            _currentOutline.enabled = true;
        }
    }

    public override void QuitPuzzle()
    {
        base.QuitPuzzle();
        TutorialManager.TutorialManagerEvents.stopTutorial();
        foreach (var scroll in radioScrolls)
        {
            _currentOutline = scroll.gameObject.GetComponent<Outline>();
            if (_currentOutline != null)
            {
                 Destroy(_currentOutline);
            }
        }
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
            if (isAfterLoad)
            {
                if (isFinished)
                {
                    count = 2;
                }
            }

            Sound soundToPlay = count == 1 ? Sound.RadioDistCode : 
                count >= 2 ? Sound.RadioCode : Sound.None;

            if (soundToPlay != _currentSecondarySound)
            {
                HandleSoundTransition(soundToPlay, count);
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
    
    private void HandleSoundTransition(Sound newSound, int currentNeedleCount)
    {
        // Stop previous sound
        if (_currentSecondarySound != Sound.None)
        {
            SoundManager.StopSound3D(_currentSecondarySound, transform);
            
            if (_codePlayCoroutine != null)
            {
                StopCoroutine(_codePlayCoroutine);
                _codePlayCoroutine = null;
                _isCodePlaying = false;
            }
        }
        
        // Play new sound only if needle count matches
        if (newSound != Sound.None)
        {
            bool shouldPlay = (newSound == Sound.RadioDistCode && currentNeedleCount == 1) ||
                              (newSound == Sound.RadioCode && currentNeedleCount >= 2);
            
            if (shouldPlay)
            {
                _codePlayCoroutine = StartCoroutine(PlayCodeRepeating(newSound, currentNeedleCount));
                _currentSecondarySound = newSound;
            }
            else
            {
                _currentSecondarySound = Sound.None;
            }
        }
        else
        {
            _currentSecondarySound = Sound.None;
        }
    }

    private IEnumerator PlayCodeRepeating(Sound repeatSound, int requiredNeedleCount)
    {
        while (GetCorrectNeedleCount() == requiredNeedleCount || (isAfterLoad && isFinished))
        {
            if (!_isCodePlaying)
            {
                _isCodePlaying = true;
                SoundManager.PlaySound3D(repeatSound, transform, null, codeVolume);
                
                float clipLength = GetAudioClipLength(repeatSound);
                yield return new WaitForSeconds(clipLength);
                
                _isCodePlaying = false;
                
                // Only wait the delay if we're still in the correct state
                if (GetCorrectNeedleCount() == requiredNeedleCount)
                {
                    yield return new WaitForSeconds(codeRepeatDelay);
                }
            }
            else
            {
                yield return null;
            }
        }
        
        // If we exited the loop, stop the sound
        SoundManager.StopSound3D(repeatSound, transform);
        _isCodePlaying = false;
        _currentSecondarySound = Sound.None;
    }
    
    public static class RadioEvents
    {
        public static Action CheckNeedles;
        public static Func<bool> GetIsFinished;
    }
}