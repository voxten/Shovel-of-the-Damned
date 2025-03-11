using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Library", menuName = "ScriptableObjects/AudioLibrary", order = 0)]
public class AudioLibraryData : ScriptableObject {
    [SerializeField] private List<SoundPair> sounds;

    public AudioClip GetAudioClip(Sound _type) {
        var soundList = sounds.Where(_sfxPair => _sfxPair.type == _type)
            .Select(_sfxPair => _sfxPair.clip).FirstOrDefault();
        return soundList?[Random.Range(0, soundList.Count)];
    }
        
}

[Serializable]
public struct SoundPair {
    public List<AudioClip> clip;
    public Sound type;
}
