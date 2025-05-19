using UnityEngine;

public class LoopableSound : MonoBehaviour
{
    private void Start()
    {
        SoundManager.PlaySound3D(Sound.SparksLoop, transform, null, 0.1f);
    }
}