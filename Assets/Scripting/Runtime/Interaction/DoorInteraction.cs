using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoorInteraction : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private Sound openSound;
    [SerializeField] private Sound closeSound;
    
    private bool _isOpen;
    private const float ClosedDoorY = 0f;
    private const float OpenedDoorY = 2f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ToggleDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ToggleDoor();
        }
    }

    private void ToggleDoor()
    {
        if (_isOpen)
        {
            door.transform.DOLocalMoveY(ClosedDoorY, 0.5f).SetEase(Ease.InOutSine);
            SoundManager.PlaySound3D(closeSound, door.transform, new Vector2(.9f,1.1f), new Vector2(.1f,.2f));
        }
        else
        {
            door.transform.DOLocalMoveY(OpenedDoorY, 0.5f).SetEase(Ease.InOutSine);
            SoundManager.PlaySound3D(openSound, door.transform, new Vector2(.9f,1.1f), new Vector2(.1f,.2f));
        }
        _isOpen = !_isOpen;
    }
}
