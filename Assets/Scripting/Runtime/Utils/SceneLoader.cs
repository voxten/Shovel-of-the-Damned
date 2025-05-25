using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private float fadeDuration = 0.5f;
    private CanvasGroup _canvasGroup;

    private void OnEnable()
    {
        SceneEvents.AnimateLoadScene += AnimateLoadScene;
    }

    private void OnDisable()
    {
        SceneEvents.AnimateLoadScene -= AnimateLoadScene;
    }

    private void Awake()
    {
        _canvasGroup = loadingScreen.GetComponent<CanvasGroup>();
        // Ensure the loading screen is initially transparent
        _canvasGroup.alpha = 1f;
        loadingScreen.SetActive(true);
        
        _canvasGroup.DOFade(0f, fadeDuration)
            .OnComplete(() => 
            {
                loadingScreen.SetActive(false);
            });
    }

    private void AnimateLoadScene(string sceneName)
    {
        // Activate loading screen
        loadingScreen.SetActive(true);
        _canvasGroup.alpha = 0f;
        
        // Fade in
        _canvasGroup.DOFade(1f, fadeDuration)
            .OnComplete(() =>
            {
                // Start loading the scene asynchronously after fade in
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
                
                // If you want to wait for the scene to fully load before fading out:
                if (asyncLoad != null)
                    asyncLoad.completed += (operation) =>
                    {
                        // Fade out (optional - if you want to fade out after the new scene is loaded)
                        _canvasGroup.DOFade(0f, fadeDuration)
                            .OnComplete(() => { loadingScreen.SetActive(false); Utilitis.SetCursorState(false); });
                    };
            });
    }

    public static class SceneEvents
    {
        public static Action<string> AnimateLoadScene;
    }
}