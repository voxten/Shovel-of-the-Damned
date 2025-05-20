using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayButton : ExtendedButton
{
    [SerializeField] private AudioSource menuMusic;
    
    protected override void Submit()
    {
        Utilitis.SetCursorState(true);
        SceneLoader.SceneEvents.AnimateLoadScene("CarScene");
        StartCoroutine(FadeOutAndLoadScene());
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        float duration = 2f;
        float startVolume = menuMusic.volume;

        float time = 0f;
        while (time < duration)
        {
            menuMusic.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        menuMusic.volume = 0f;
    }
}