using UnityEngine;

public class LoadButton : ExtendedButton
{
    protected override void Submit()
    {
        base.Submit();
        SavingSystem.SavingSystemEvents.Load();
    }
}
