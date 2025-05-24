using UnityEngine;

public class MenuButton : ExtendedButton
{
    protected override void Submit()
    {
        base.Submit();
        Time.timeScale = 1f;
        SceneLoader.SceneEvents.AnimateLoadScene("MainMenu");
    }
}
