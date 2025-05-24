using UnityEngine;

public class LoadButton : ExtendedButton
{
    protected override void Submit()
    {
        base.Submit();
        Time.timeScale = 1f;
        SavingSystem.SavingSystemEvents.Load();
    }
}
