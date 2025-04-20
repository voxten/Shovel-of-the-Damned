using UnityEngine;

public class ExtendedPanelButton : ExtendedButton
{
    [SerializeField] protected GameObject panelToClose;
    [SerializeField] protected GameObject panelToOpen;
    
    protected override void Submit()
    {
        base.Submit();
        panelToClose.SetActive(false);
        panelToOpen.SetActive(true);
    }
}