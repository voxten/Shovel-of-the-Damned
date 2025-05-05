using System;
using UnityEngine;
using DG.Tweening;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private AccessCardType requiredAccessLevel;
    [SerializeField] private GameObject doorObject;
    [SerializeField] private float animationDuration = 1f;
    
    private float _doorMaxY = 395f;
    private float _doorMinY = 0f;
    private bool _isOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var card = Inventory.InventoryEvents.GetAccessCard();

            if (card is AccessCardItem cardItem)
            {
                // Check if player's card level is equal or higher than required
                if (cardItem.cardPair.cardType >= requiredAccessLevel)
                {
                    OpenDoor();
                }
                else
                {
                    Debug.Log($"Access denied! Required: {requiredAccessLevel}, Current: {cardItem.cardPair.cardType}");
                }
            }
            else
            {
                Debug.Log("No access card found!");
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
}