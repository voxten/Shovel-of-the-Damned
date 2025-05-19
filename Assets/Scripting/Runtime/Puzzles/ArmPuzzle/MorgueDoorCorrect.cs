using System;
using UnityEngine;
using DG.Tweening; // Required for DOTween

public class MorgueDoorCorrect : InteractableObject
{
    [SerializeField] private Item morgueKey;
    [SerializeField] private GameObject morgueTakhtObject;
    [SerializeField] private float doorAnimationDuration = 1f;
    [SerializeField] private float drawerAnimationDuration = 1f;
    [SerializeField] private Ease doorEaseType = Ease.OutBack;
    [SerializeField] private Ease drawerEaseType = Ease.OutSine;
    
    private GameObject _morgueDoorObject;
    private Collider _morgueCollider;

    private float _morgueDoorYMin = 0f;
    private float _morgueDoorYMax = 96f;

    private float _takhtXMin = 0f;
    private float _takhtXMax = -1.9f;

    private void Awake()
    {
        _morgueDoorObject = gameObject;
        _morgueCollider = GetComponent<Collider>();
    }

    public override bool Interact()
    {
        if (Inventory.InventoryEvents.FindItem(morgueKey))
        {
            Inventory.InventoryEvents.RemoveItem(morgueKey);
            OpenDoor();
            SoundManager.PlaySound3D(Sound.MorgueDoorKey, transform);
            Narration.DisplayText?.Invoke("It worked...");
        }
        else
        {
            SoundManager.PlaySound3D(Sound.MorgueDoorTry, transform);
            Narration.DisplayText?.Invoke("It’s sealed tight...");
        }
        return true;
    }

    private void OpenDoor()
    {
        //SoundManager.PlaySound3D(Sound.MorgueDoorOpen, transform);
        _morgueDoorObject.transform.DOLocalRotate(
                new Vector3(0, _morgueDoorYMax, 0),
                doorAnimationDuration,
                RotateMode.Fast
            ).SetEase(doorEaseType)
            .OnComplete(PullDrawer);
    }

    private void PullDrawer()
    {
        // Move the drawer using DOTween
        SoundManager.PlaySound3D(Sound.MorgueDrawer, transform);
        morgueTakhtObject.transform.DOLocalMoveX(
            _takhtXMax,
            drawerAnimationDuration
        ).SetEase(drawerEaseType).OnComplete(()=> _morgueCollider.enabled = false);
    }
}