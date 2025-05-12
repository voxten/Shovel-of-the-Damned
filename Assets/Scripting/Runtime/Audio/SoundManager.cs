using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Audio; // Added for AudioMixerGroup

public class SoundManager : MonoBehaviour 
{
    [SerializeField] private AudioSource mainAudio;
    [SerializeField] private AudioSource musicAudio;
    [SerializeField] private AudioLibraryData audioLibrary;
    [SerializeField] private AudioMixerGroup mainMixerGroup; // Assign in inspector
    [SerializeField] private AudioMixerGroup sfxMixerGroup; // Assign in inspector

    private static Action<Sound, Vector2?, Vector2?> _onPlaySound;
    private static Action<Sound, Transform, Vector2?, Vector2?> _onPlaySound3D;
    private static Action<Sound> _onStopSound; // Delegate for stopping sound

    private static Dictionary<Sound, AudioSource> activeSounds = new();

    private void OnEnable() 
    {
        _onPlaySound += PlayAudioShot;
        _onPlaySound3D += Play3DAudioShot;
        _onStopSound += StopAudioShot;
    }

    private void OnDisable() 
    {
        _onPlaySound -= PlayAudioShot;
        _onPlaySound3D -= Play3DAudioShot;
        _onStopSound -= StopAudioShot;
    }

    public static void PlaySound(Sound type, Vector2? pitchRange = null, Vector2? volumeRange = null) 
    {
        if (_onPlaySound == null) Debug.LogError("There is no SoundManager in the scene");
        _onPlaySound?.Invoke(type, pitchRange, volumeRange);
    }

    public static void PlaySound3D(Sound type, Transform objTransform, Vector2? pitchRange = null, Vector2? volumeRange = null) 
    {
        if (_onPlaySound3D == null) Debug.LogError("There is no SoundManager in the scene");
        _onPlaySound3D?.Invoke(type, objTransform, pitchRange, volumeRange);
    }

    public static void StopSound(Sound type)
    {
        if (_onStopSound == null) Debug.LogError("There is no SoundManager in the scene");
        _onStopSound?.Invoke(type);
    }

    private void Play3DAudioShot(Sound type, Transform objTransform, Vector2? pitchRange = null, Vector2? volumeRange = null) 
    {
        if (type == Sound.None) return;
    
        var sfx = audioLibrary.GetAudioClip(type);
        if (sfx == null) return;

        // Create a temporary GameObject for the audio
        GameObject tempAudioObject = new GameObject("TempAudio_" + type);
        tempAudioObject.transform.position = objTransform.position;
    
        // Add AudioSource to the temporary GameObject
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = sfxMixerGroup;
        audioSource.clip = sfx;
        audioSource.spatialBlend = 1f; // Makes it 3D
        audioSource.volume = volumeRange != null ? Random.Range(volumeRange.Value.x, volumeRange.Value.y) : 1f;
        audioSource.pitch = pitchRange != null ? Random.Range(pitchRange.Value.x, pitchRange.Value.y) : 1f;
        audioSource.loop = false;
        audioSource.Play();

        // Destroy the temporary object after the clip finishes playing
        Destroy(tempAudioObject, sfx.length + 0.1f); // Adding a small buffer
    }

    private void PlayAudioShot(Sound type, Vector2? pitchRange = null, Vector2? volumeRange = null) 
    {
        if (type == Sound.None) return;
        
        var sfx = audioLibrary.GetAudioClip(type);
        AudioSource source = mainAudio; // Default to mainAudio

        if (activeSounds.ContainsKey(type)) 
        {
            source = activeSounds[type]; // Use existing source if available
        }
        else
        {
            // Determine which mixer group to use based on sound type
            // You might want to add logic here to decide between main and SFX mixer
            // For now, I'm assuming all 2D sounds go to main mixer
            source.outputAudioMixerGroup = mainMixerGroup;
            activeSounds[type] = source;
        }

        source.clip = sfx;
        source.volume = volumeRange != null ? Random.Range(volumeRange.Value.x, volumeRange.Value.y) : 1f;
        source.pitch = pitchRange != null ? Random.Range(pitchRange.Value.x, pitchRange.Value.y) : 1.5f;
        source.loop = false;
        source.Play();
    }

    private void StopAudioShot(Sound type)
    {
        if (activeSounds.TryGetValue(type, out AudioSource source))
        {
            source.Stop();
        }
    }
}