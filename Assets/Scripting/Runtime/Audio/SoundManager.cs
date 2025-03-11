using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour {
    [SerializeField] private AudioSource mainAudio;
    [SerializeField] private AudioSource musicAudio;

    [SerializeField] private AudioLibraryData audioLibrary;

    private static Action<Sound, Vector2?, Vector2?> onPlaySound;
    private static Action<Sound, Transform, Vector2?, Vector2?> onPlaySound3D;

    private void OnEnable() {
        onPlaySound += PlayAudioShot;
        onPlaySound3D += Play3DAudioShot;
    }

    private void OnDisable() {
        onPlaySound -= PlayAudioShot;
        onPlaySound3D -= Play3DAudioShot;
    }

    public static void PlaySound(Sound _type, Vector2? _pitchRange = null, Vector2? _volumeRange = null) {
        if (onPlaySound == null) Debug.LogError("There is no SoundManager in the scene");

        onPlaySound?.Invoke(_type, _pitchRange, _volumeRange);
    }

    public static void PlaySound3D(Sound _type, Transform _transform, Vector2? _pitchRange = null,
        Vector2? _volumeRange = null) {
        if (onPlaySound3D == null) Debug.LogError("There is no SoundManager in the scene");

        onPlaySound3D?.Invoke(_type, _transform, _pitchRange, _volumeRange);
    }

    private void Play3DAudioShot(Sound _type, Transform _transform, Vector2? _pitchRange = null,
        Vector2? _volumeRange = null) {
        HandleAudioPitch(_pitchRange);
        HandleAudioVolume(_volumeRange);

        var sfx = audioLibrary.GetAudioClip(_type);
        if(_volumeRange != null)
            AudioSource.PlayClipAtPoint(sfx, _transform.position, Random.Range(_volumeRange.Value.x, _volumeRange.Value.y));
        else
            AudioSource.PlayClipAtPoint(sfx, _transform.position);
    }

    private void PlayAudioShot(Sound _type, Vector2? _pitchRange = null, Vector2? _volumeRange = null) {
        HandleAudioPitch(_pitchRange);
        HandleAudioVolume(_volumeRange);

        var sfx = audioLibrary.GetAudioClip(_type);
        if(_volumeRange != null)
            mainAudio.PlayOneShot(sfx, Random.Range(_volumeRange.Value.x, _volumeRange.Value.y));
        else
            mainAudio.PlayOneShot(sfx);
    }

    private void HandleAudioVolume(Vector2? _volumeRange) {
        if (_volumeRange != null)
            mainAudio.volume = Random.Range(_volumeRange.Value.x, _volumeRange.Value.y);
        else
            mainAudio.volume = 1;
    }

    private void HandleAudioPitch(Vector2? _pitchRange) {
        if (_pitchRange != null)
            mainAudio.pitch = Random.Range(_pitchRange.Value.x, _pitchRange.Value.y);
        else
            mainAudio.pitch = 1.5f;
    }
}