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
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Sound paperSound;
    private NoteItem _currentNoteItem;
    private bool _isOn;
    private int _currentIndex;

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
        _currentIndex = 0;
        _isOn = true;
        _currentNoteItem = noteItem;
        noteText.text = noteItem.noteContent[_currentIndex];
        closeButton.onClick.AddListener(ClosePanel);
        notePanel.SetActive(true);
        
        if (noteItem.noteContent.Count > 1)
        {
            nextButton.onClick.AddListener(NextNote);
            previousButton.onClick.AddListener(PreviousNote);
            UpdateNavigationButtons();
        }
        SoundManager.PlaySound(paperSound);
        ToggleUtils(true);
    }

    private void NextNote()
    {
        if (_currentIndex + 1 < _currentNoteItem.noteContent.Count)
        {
            _currentIndex++;
            noteText.text = _currentNoteItem.noteContent[_currentIndex];
            UpdateNavigationButtons();
        }
    }

    private void PreviousNote()
    {
        if (_currentIndex - 1 >= 0)
        {
            _currentIndex--;
            noteText.text = _currentNoteItem.noteContent[_currentIndex];
            UpdateNavigationButtons();
        }
    }

    private void UpdateNavigationButtons()
    {
        SoundManager.PlaySound(paperSound);
        nextButton.gameObject.SetActive(_currentIndex < _currentNoteItem.noteContent.Count - 1);
        previousButton.gameObject.SetActive(_currentIndex > 0);
    }

    private void ClosePanel()
    {
        _isOn = false;
        noteText.text = "";
        closeButton.onClick.RemoveAllListeners();
        previousButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();
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
