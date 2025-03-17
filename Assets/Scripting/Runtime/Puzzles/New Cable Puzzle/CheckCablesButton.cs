using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CheckCablesButton : InteractableObject
{
    [Header("Objects")]
    [SerializeField] private List<NewCable> cables;
    [SerializeField] private GameObject fuseBoxDoor;
    
    [SerializeField] private GameObject fuse;
    private Rigidbody _fuseRigibody;
    private Collider _fuseCollider;
    
    [Header("Audio")]
    [SerializeField] private Sound checkSound;
    [SerializeField] private Sound finishSound;
    [SerializeField] private Sound finishLoopSound;
    [SerializeField] private Sound fuseBoxOpenSound;
    
    private const float StandardButton = 0.039f;  
    private const float PressedButton = 0.029f;
    
    private bool _isFinished;
    private int _correctCables;

    private void Awake()
    {
        _fuseRigibody = fuse.gameObject.GetComponent<Rigidbody>();
        _fuseCollider = fuse.gameObject.GetComponent<Collider>();
    }

    private void OnEnable()
    {
        CablePuzzle.GetFinished += GetFinished;
    }

    private void OnDisable()
    {
        CablePuzzle.GetFinished -= GetFinished;
    }

    public override bool Interact()
    {
        if (!_isFinished)
        {
            CheckCables();
        }
        return true;
    }

    private void CheckCables()
    {
        SoundManager.PlaySound3D(checkSound, transform);
        AnimateButton();
        foreach (var cable in cables)
        {
            if (cable.CheckCable())
            {
                cable.SetLight();
                _correctCables++;
            }
        }

        if (_correctCables == 4)
        {
            _isFinished = true;
            StartCoroutine(WaitForLoopable());
        }
        else
        {
            Narration.DisplayText?.Invoke("I did something wrong...");
            _correctCables = 0;
        }
    }

    private bool GetFinished()
    {
        return _isFinished;
    }

    private IEnumerator WaitForLoopable()
    {
        Narration.DisplayText?.Invoke("Something has opened up beneath.");
        SoundManager.PlaySound3D(finishSound, transform);
        yield return new WaitForSeconds(1);
        SetFuse();
        OpenBox();
        yield return new WaitForSeconds(7);
        SoundManager.PlaySound3D(finishLoopSound, transform);
    }

    private void OpenBox()
    {
        SoundManager.PlaySound3D(fuseBoxOpenSound, fuseBoxDoor.transform);
        fuseBoxDoor.transform.DOLocalRotate(new Vector3(0, 125, 0), 1.0f, RotateMode.FastBeyond360);
    }

    private void SetFuse()
    {
        _fuseCollider.enabled = true;
        _fuseRigibody.isKinematic = false;
    }

    private void AnimateButton()
    {
        var position = transform.localPosition;
            
        gameObject.transform.DOLocalMove(new Vector3(position.x, PressedButton, position.z), 0.25f)
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                gameObject.transform.DOLocalMove(new Vector3(position.x, StandardButton, position.z), 0.25f)
                    .SetEase(Ease.OutExpo);
            });
    }

    public static class CablePuzzle 
    {
        public static Func<bool> GetFinished;
    }
}