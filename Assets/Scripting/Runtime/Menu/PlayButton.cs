using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : ExtendedButton
{
    protected override void Submit()
    {
        SceneManager.LoadScene("MainScene");
    }
}
