using UnityEngine;
using System;
using StarterAssets;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    private bool _isOn;

    private void OnEnable()
    {
        InventoryUIEvents.GetIsOn += GetIsOn;
    }

    private void OnDisable()
    {
        InventoryUIEvents.GetIsOn -= GetIsOn;
    }

    private void Update()
    {
        if (PlayerDeathUIPlayerDeathUIManager.DeathEvents.GetIsPlayerDead()) return;
        if (Input.GetKeyDown(KeyCode.Tab) && !NoteUIManager.NoteActions.GetIsOn() && !PauseManager.PauseEvents.GetIsOn())
        {
            inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
            _isOn = inventoryUI.gameObject.activeSelf;
        }
    }

    private bool GetIsOn()
    {
        return _isOn;
    }
    
    public static class InventoryUIEvents
    {
        public static Func<bool> GetIsOn;
    }
}
