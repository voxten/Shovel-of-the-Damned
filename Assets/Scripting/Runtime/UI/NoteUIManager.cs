using System;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteUIManager : MonoBehaviour
{
    [Header("Note")]
    [SerializeField] private GameObject notePanel;
    [SerializeField] private TextMeshProUGUI noteText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    private NoteItem _currentNoteItem;
    
    [Header("Picture Note")]
    [SerializeField] private GameObject picturePanel;
    [SerializeField] private Image pictureImage;
    [SerializeField] private Button closePictureButton;
    
    private bool _isOn;
    private int _currentIndex;

    private void OnEnable()
    {
        NoteActions.OpenNote += OpenNote;
        NoteActions.OpenPicture += OpenPicture;
        NoteActions.GetIsOn += GetIsOn;
        NoteActions.CloseNotePanel += CloseNotePanel;
    }

    private void OnDisable()
    {
        NoteActions.OpenNote -= OpenNote;
        NoteActions.OpenPicture -= OpenPicture;
        NoteActions.GetIsOn -= GetIsOn;
        NoteActions.CloseNotePanel -= CloseNotePanel;
    }

    private void OpenNote(NoteItem noteItem)
    {
        if (noteItem.noteContent != null)
        {
            TutorialManager.TutorialManagerEvents.stopTutorial();
        }
        
        _currentIndex = 0;
        _isOn = true;
        _currentNoteItem = noteItem;
        notePanel.SetActive(true);
        if (noteItem.noteContent != null)
        {
            noteText.text = noteItem.noteContent[_currentIndex];
            SoundManager.PlaySound(Sound.Paper);
        }
        else
        {
            CloseNotePanel();
        }
        closeButton.onClick.AddListener(CloseNotePanel);
        
        
        if (noteItem.noteContent != null && noteItem.noteContent.Count > 1)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.onClick.AddListener(NextNote);
            previousButton.onClick.AddListener(PreviousNote);
            UpdateNavigationButtons();
        }
        else
        {
            previousButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
        }
        SoundManager.PlaySound(Sound.Paper);
        ToggleUtils(true);
    }
    
    private void OpenPicture(PictureItem pictureItem)
    {
        TutorialManager.TutorialManagerEvents.stopTutorial();
        _currentIndex = 0;
        _isOn = true;
        pictureImage.sprite = pictureItem.pictureSprite;
        closePictureButton.onClick.AddListener(ClosePicturePanel);
        picturePanel.SetActive(true);
        
        SoundManager.PlaySound(Sound.Paper);
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
        SoundManager.PlaySound(Sound.Paper);
        nextButton.gameObject.SetActive(_currentIndex < _currentNoteItem.noteContent.Count - 1);
        previousButton.gameObject.SetActive(_currentIndex > 0);
    }

    private void CloseNotePanel()
    {
        _isOn = false;
        noteText.text = "";
        closeButton.onClick.RemoveAllListeners();
        previousButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();
        notePanel.SetActive(false);
        ToggleUtils(false);
    }
    
    private void ClosePicturePanel()
    {
        _isOn = false;
        closePictureButton.onClick.RemoveAllListeners();
        picturePanel.SetActive(false);
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
        public static Action<PictureItem> OpenPicture;
        public static Func<bool> GetIsOn;
        public static Action CloseNotePanel;
    }
}
