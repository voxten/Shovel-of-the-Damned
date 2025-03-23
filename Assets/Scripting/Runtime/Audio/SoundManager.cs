using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour 
{
    [SerializeField] private AudioSource mainAudio;
    [SerializeField] private AudioSource musicAudio;

    [SerializeField] private AudioLibraryData audioLibrary;

    private static Action<Sound, Vector2?, Vector2?> _onPlaySound;
    private static Action<Sound, Transform, Vector2?, Vector2?> _onPlaySound3D;

    private void OnEnable() 
    {
        _onPlaySound += PlayAudioShot;
        _onPlaySound3D += Play3DAudioShot;
    }

    private void OnDisable() 
    {
        _onPlaySound -= PlayAudioShot;
        _onPlaySound3D -= Play3DAudioShot;
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

    private void Play3DAudioShot(Sound type, Transform objTransform, Vector2? pitchRange = null, Vector2? volumeRange = null) 
    {
        HandleAudioPitch(pitchRange);
        HandleAudioVolume(volumeRange);

        if (type == Sound.None) 
            return;
        
        var sfx = audioLibrary.GetAudioClip(type);
        if(volumeRange != null)
            AudioSource.PlayClipAtPoint(sfx, objTransform.position, Random.Range(volumeRange.Value.x, volumeRange.Value.y));
        else
            AudioSource.PlayClipAtPoint(sfx, objTransform.position);
    }

    private void PlayAudioShot(Sound type, Vector2? pitchRange = null, Vector2? volumeRange = null) 
    {
        HandleAudioPitch(pitchRange);
        HandleAudioVolume(volumeRange);

        var sfx = audioLibrary.GetAudioClip(type);
        if(volumeRange != null)
            mainAudio.PlayOneShot(sfx, Random.Range(volumeRange.Value.x, volumeRange.Value.y));
        else
            mainAudio.PlayOneShot(sfx);
    }

    private void HandleAudioVolume(Vector2? volumeRange) 
    {
        if (volumeRange != null)
            mainAudio.volume = Random.Range(volumeRange.Value.x, volumeRange.Value.y);
        else
            mainAudio.volume = 1;
    }

    private void HandleAudioPitch(Vector2? pitchRange) 
    {
        if (pitchRange != null)
            mainAudio.pitch = Random.Range(pitchRange.Value.x, pitchRange.Value.y);
        else
            mainAudio.pitch = 1.5f;
    }
}