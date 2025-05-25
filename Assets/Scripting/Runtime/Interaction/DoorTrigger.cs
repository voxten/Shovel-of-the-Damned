using System;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private AccessCardType requiredAccessLevel;
    [SerializeField] private GameObject doorObject;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private TextMeshPro[] numberTexts;

    public bool canOpen = true;
    
    private Color _approvedColor = Color.green;
    private Color _deniedColor = Color.red;
    private Color _neutralColor = Color.white;
    
    private float _doorMaxY = 395f;
    private float _doorMinY = 0f;
    private bool _isOpen;

    private void Awake()
    {
        SetTextColor(_neutralColor);
        var number = SetMonitorNumber();
        foreach (var numberText in numberTexts)
        {
            numberText.text = number;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canOpen) return;
        
        if (other.CompareTag("Player"))
        {
            var card = Inventory.InventoryEvents.GetAccessCard();

            if (card is AccessCardItem cardItem)
            {
                // Check if player's card level is equal or higher than required
                if (cardItem.cardPair.cardType >= requiredAccessLevel)
                {
                    OpenDoor();
                    SoundManager.PlaySound3D(Sound.CodeApprove, transform);
                    SetTextColor(_approvedColor);
                }
                else
                {
                    Debug.Log($"Access denied! Required: {requiredAccessLevel}, Current: {cardItem.cardPair.cardType}");
                    SoundManager.PlaySound3D(Sound.CodeDenied, transform);
                    SetTextColor(_deniedColor);
                }
            }
            else
            {
                Debug.Log("No access card found!");
                SoundManager.PlaySound3D(Sound.CodeDenied, transform);
                SetTextColor(_deniedColor);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            // Enemies can always pass
            OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            CloseDoor();
            SetTextColor(_neutralColor);
        }
    }

    private void OpenDoor()
    {
        if (_isOpen) return;
        
        _isOpen = true;
        SoundManager.PlaySound3D(Sound.DoorOpen, transform);
        doorObject.transform.DOLocalMoveY(_doorMaxY, animationDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => Debug.Log("Door fully opened"));
    }

    private void CloseDoor()
    {
        if (!_isOpen) return;
        
        _isOpen = false;
        SoundManager.PlaySound3D(Sound.DoorClose, transform);
        doorObject.transform.DOLocalMoveY(_doorMinY, animationDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => Debug.Log("Door fully closed"));
    }

    private void SetTextColor(Color color)
    {
        foreach (var numberText in numberTexts)
        {
            numberText.color = color;
        }
    }

    private string SetMonitorNumber()
    {
        switch (requiredAccessLevel)
        {
            case AccessCardType.Level0:
                return "0";
            case AccessCardType.Level1:
                return "1";
            case AccessCardType.Level2:
                return "2";
            case AccessCardType.Level3:
                return "3";
            case AccessCardType.Level4:
                return "4";
            default:
                return "None";
        }
    }
}