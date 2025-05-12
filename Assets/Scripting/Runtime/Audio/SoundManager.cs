using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour 
{
    [SerializeField] private AudioSource mainAudio;
    [SerializeField] private AudioSource musicAudio;
    [SerializeField] private AudioLibraryData audioLibrary;
    [SerializeField] private AudioMixerGroup mainMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private static Action<Sound, Vector2?, Vector2?> _onPlaySound;
    private static Action<Sound, Transform, Vector2?, Vector2?> _onPlaySound3D;
    private static Action<Sound> _onStopSound;
    private static Action<Sound, Transform> _onStopSound3D; // New delegate for stopping 3D sounds

    private static Dictionary<Sound, AudioSource> activeSounds = new();
    private static Dictionary<Sound, List<GameObject>> active3DSounds = new(); // Track 3D sound objects

    private void OnEnable() 
    {
        _onPlaySound += PlayAudioShot;
        _onPlaySound3D += Play3DAudioShot;
        _onStopSound += StopAudioShot;
        _onStopSound3D += Stop3DAudioShot; // Register new delegate
    }

    private void OnDisable() 
    {
        _onPlaySound -= PlayAudioShot;
        _onPlaySound3D -= Play3DAudioShot;
        _onStopSound -= StopAudioShot;
        _onStopSound3D -= Stop3DAudioShot; // Unregister
    }

    public static void PlaySound(Sound type, Vector2? pitchRange = null, Vector2? volumeRange = null) 
    {
        if (_onPlaySound == null) Debug.LogError("There is no SoundManager in the scene");
        _onPlaySound?.Invoke(type, pitchRange, volumeRange);
    }

    public static void PlaySound3D(Sound type, Transform objTransform, float volume = 1f, float pitch = 1f)
    {
        if (_onPlaySound3D == null) Debug.LogError("There is no SoundManager in the scene");
        _onPlaySound3D?.Invoke(type, objTransform, null, new Vector2(volume, volume));
    }

    public static void StopSound(Sound type)
    {
        if (_onStopSound == null) Debug.LogError("There is no SoundManager in the scene");
        _onStopSound?.Invoke(type);
    }

    public static void StopSound3D(Sound type, Transform objTransform)
    {
        if (_onStopSound3D == null) Debug.LogError("There is no SoundManager in the scene");
        _onStopSound3D?.Invoke(type, objTransform);
    }
    
    private void Play3DAudioShot(Sound type, Transform objTransform, Vector2? pitchRange = null, Vector2? volumeRange = null) 
    {
        if (type == Sound.None) return;

        var sfx = audioLibrary.GetAudioClip(type);
        if (sfx == null) return;

        GameObject tempAudioObject = new GameObject("TempAudio_" + type);
        tempAudioObject.transform.position = objTransform.position;
        tempAudioObject.transform.parent = objTransform; // Make it follow the transform
    
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = sfxMixerGroup;
        audioSource.clip = sfx;
        audioSource.spatialBlend = 1f;
    
        // Set volume (use single value if provided, otherwise random range)
        if (volumeRange.HasValue && volumeRange.Value.x == volumeRange.Value.y)
            audioSource.volume = volumeRange.Value.x;
        else
            audioSource.volume = volumeRange != null ? Random.Range(volumeRange.Value.x, volumeRange.Value.y) : 1f;
    
        // Set pitch (use single value if provided, otherwise random range)
        if (pitchRange.HasValue && pitchRange.Value.x == pitchRange.Value.y)
            audioSource.pitch = pitchRange.Value.x;
        else
            audioSource.pitch = pitchRange != null ? Random.Range(pitchRange.Value.x, pitchRange.Value.y) : 1f;
    
        audioSource.loop = type.ToString().Contains("Loop");
        audioSource.Play();

        if (!active3DSounds.ContainsKey(type))
        {
            active3DSounds[type] = new List<GameObject>();
        }
        active3DSounds[type].Add(tempAudioObject);

        if (!audioSource.loop)
        {
            Destroy(tempAudioObject, sfx.length + 0.1f);
            active3DSounds[type].Remove(tempAudioObject);
        }
    }

    private void PlayAudioShot(Sound type, Vector2? pitchRange = null, Vector2? volumeRange = null) 
    {
        if (type == Sound.None) return;
        
        var sfx = audioLibrary.GetAudioClip(type);
        AudioSource source = mainAudio;

        if (activeSounds.ContainsKey(type)) 
        {
            source = activeSounds[type];
        }
        else
        {
            source.outputAudioMixerGroup = mainMixerGroup;
            activeSounds[type] = source;
        }

        source.clip = sfx;
        source.volume = volumeRange != null ? Random.Range(volumeRange.Value.x, volumeRange.Value.y) : 1f;
        source.pitch = pitchRange != null ? Random.Range(pitchRange.Value.x, pitchRange.Value.y) : 1.5f;
        source.loop = type.ToString().Contains("Loop");
        source.Play();
    }

    private void StopAudioShot(Sound type)
    {
        if (activeSounds.TryGetValue(type, out AudioSource source))
        {
            source.Stop();
        }
    }

    private void Stop3DAudioShot(Sound type, Transform objTransform)
    {
        if (active3DSounds.TryGetValue(type, out List<GameObject> soundObjects))
        {
            for (int i = soundObjects.Count - 1; i >= 0; i--)
            {
                if (soundObjects[i] != null && 
                    Vector3.Distance(soundObjects[i].transform.position, objTransform.position) < 0.1f)
                {
                    Destroy(soundObjects[i]);
                    soundObjects.RemoveAt(i);
                }
            }
        }
    }
}