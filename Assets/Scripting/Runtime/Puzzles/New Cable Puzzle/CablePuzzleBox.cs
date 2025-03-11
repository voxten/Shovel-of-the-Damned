using System;
using UnityEngine;
using DG.Tweening;

public class CablePuzzleBox : InteractableObject
{
    [SerializeField] private GameObject door;
    [SerializeField] private Sound fuseBoxOpenSound;
    private bool _isFirst = true;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public override bool Interact()
    {
        if (_isFirst)
        {
            door.transform.DOLocalRotate(new Vector3(0, 125, 0), 1.0f, RotateMode.FastBeyond360);
            SoundManager.PlaySound3D(fuseBoxOpenSound, transform);
            _isFirst = false;
            _collider.enabled = false;
        }
        return true;
    }
}