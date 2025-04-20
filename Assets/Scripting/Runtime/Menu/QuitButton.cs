using UnityEngine;

public class QuitButton : ExtendedButton
{
    protected override void Submit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
