using System;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteUIManager : MonoBehaviour
{
    [SerializeField] private GameObject notePanel;
    [SerializeField] private TextMeshProUGUI noteText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Sound paperSound;
    private bool _isOn;

    private void OnEnable()
    {
        NoteActions.OpenNote += OpenNote;
        NoteActions.GetIsOn += GetIsOn;
    }

    private void OnDisable()
    {
        NoteActions.OpenNote -= OpenNote;
        NoteActions.GetIsOn -= GetIsOn;
    }

    private void OpenNote(NoteItem noteItem)
    {
        _isOn = true;
        noteText.text = noteItem.noteContent;
        closeButton.onClick.AddListener(ClosePanel);
        notePanel.SetActive(true);
        SoundManager.PlaySound(paperSound);
        ToggleUtils(true);
    }

    private void ClosePanel()
    {
        _isOn = false;
        noteText.text = "";
        closeButton.onClick.RemoveAllListeners();
        notePanel.SetActive(false);
        ToggleUtils(false);
    }

    private void ToggleUtils(bool state)
    {
        if (!InventoryUIManager.InventoryUIEvents.GetIsOn())
        {
            Utilitis.SetCursorState(!state);
            FirstPersonController.PlayerEvents.ToggleMove();
            Time.timeScale = !state ? 1 : 0;
        }
    }
    
    private bool GetIsOn()
    {
        return _isOn;
    }
    
    public static class NoteActions
    {
        public static Action<NoteItem> OpenNote;
        public static Func<bool> GetIsOn;
    }
}
