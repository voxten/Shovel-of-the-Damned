using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;


public class KeyPanelPuzzle : PuzzleObject
{
    [SerializeField] private TextMeshPro passCodeText;
    [SerializeField] public String passCode;
    private String _playerPassCode;
    
    [Header("Audio")]
    [SerializeField] private Sound approvedSound;
    [SerializeField] private Sound deniedSound;
    [SerializeField] private Sound inputSound;
    [SerializeField] private Sound openDoorSound;

    private void Awake()
    {
        ClearPasscode();
    }

    private void OnEnable()
    {
        KeyPanelEvents.AddNumber += AddNumber;
        KeyPanelEvents.CheckPasscode += CheckPasscode;
        KeyPanelEvents.ClearPasscode += ClearPasscode;
    }

    private void OnDisable()
    {
        KeyPanelEvents.AddNumber -= AddNumber;
        KeyPanelEvents.CheckPasscode -= CheckPasscode;
        KeyPanelEvents.ClearPasscode -= ClearPasscode;
    }

    public override void OpenPuzzle()
    {

    }

    public override void QuitPuzzle()
    {

    }

    protected override void EndPuzzle()
    {
        base.EndPuzzle();
        //End
    }

    private void AddNumber(int number)
    {
        if (_playerPassCode.Length == 4) return;
        SoundManager.PlaySound3D(inputSound, transform);
        _playerPassCode += number.ToString();
        UpdateText();
    }

    private void CheckPasscode()
    {
        if (_playerPassCode.Length == passCode.Length)
        {
            if (_playerPassCode == passCode)
            {
                Narration.DisplayText?.Invoke("Finally I can get out from here...");
                SoundManager.PlaySound3D(approvedSound, transform);
                EndPuzzle();
            }
            else
            {
                Narration.DisplayText?.Invoke("Incorrect...");
                SoundManager.PlaySound3D(deniedSound, transform, Vector2.one, new Vector2(0.6f,0.7f));
                _playerPassCode = "";
            }
        }
        else
        {
            Narration.DisplayText?.Invoke("Passcode is to short...");
            SoundManager.PlaySound3D(deniedSound, transform, Vector2.one, new Vector2(0.6f,0.7f));
            _playerPassCode = "";
        }
        UpdateText();
    }

    private void UpdateText()
    {
        Debug.Log(_playerPassCode);
        passCodeText.text = _playerPassCode + "";
    }

    private void ClearPasscode()
    {
        passCodeText.text = "";
        _playerPassCode = "";
    }
    
    public static class KeyPanelEvents
    {
        public static Action<int> AddNumber;
        public static Action CheckPasscode;
        public static Action ClearPasscode;
    }
}
