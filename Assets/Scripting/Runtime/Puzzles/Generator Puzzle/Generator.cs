using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Generator : PuzzleObject
{
    [SerializeField] private Sound manometrSound;

    [Header("Arrows")]
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    [SerializeField] private List<GeneratorSwitch> generatorSwitches;

    private bool _isLeftOkey;
    private bool _isRightOkey;
    
    private float _currentLeftArrowRotation;
    private float _currentRightArrowRotation;
    
    private const float MaxRotation = 115f;
    private const float MinRotation = -115f;
    
    [Header("Fuse Box")] 
    [SerializeField] private GameObject fuseBoxDoor;
    
    [Header("Fuse")]
    [SerializeField] private GameObject fuse;
    [SerializeField] private Sound fuseBoxSound;
    private Rigidbody _fuseRigibody;
    private Collider _fuseCollider;

    private void OnEnable()
    {
        GeneratorSwitchEvents.CheckArrows += CheckArrows;
        GeneratorSwitchEvents.AdjustArrowRotationLeft += AdjustArrowRotationLeft;
        GeneratorSwitchEvents.AdjustArrowRotationRight += AdjustArrowRotationRight;
    }

    private void OnDisable()
    {
        GeneratorSwitchEvents.CheckArrows -= CheckArrows;
        GeneratorSwitchEvents.AdjustArrowRotationLeft -= AdjustArrowRotationLeft;
        GeneratorSwitchEvents.AdjustArrowRotationRight -= AdjustArrowRotationRight;
    }

    private void Awake()
    {
        _fuseRigibody = fuse.gameObject.GetComponent<Rigidbody>();
        _fuseCollider = fuse.gameObject.GetComponent<Collider>();
    }

    private void CheckArrows()
    {
        _isLeftOkey = Mathf.Approximately(_currentLeftArrowRotation, -10f);
        _isRightOkey = Mathf.Approximately(_currentRightArrowRotation, 70f);

        if (_isLeftOkey && _isRightOkey)
        {
            EndPuzzle();
            Narration.DisplayText?.Invoke("It worked!!");
        }

        if (!isFinished)
        {
            if (_isLeftOkey)
            {
                Narration.DisplayText?.Invoke("I need to adjust right arrow somehow...");
            }

            if (_isRightOkey)
            {
                Narration.DisplayText?.Invoke("I need to adjust left arrow somehow...");
            }
        }
    }

    public override void OpenPuzzle()
    {

    }

    public override void QuitPuzzle()
    {
        ResetGenerator();
    }

    protected override void EndPuzzle()
    {
        base.EndPuzzle();
        OpenBox();
    }

    private void AdjustArrowRotationLeft(float deltaRotation)
    {
        SoundManager.PlaySound3D(manometrSound, leftArrow.transform);
        _currentLeftArrowRotation += deltaRotation;
        _currentLeftArrowRotation = Mathf.Clamp(_currentLeftArrowRotation, MinRotation, MaxRotation);
        leftArrow.transform.DOLocalRotate(new Vector3(0, 0, _currentLeftArrowRotation), 1.0f);
    }

    private void AdjustArrowRotationRight(float deltaRotation)
    {
        SoundManager.PlaySound3D(manometrSound, rightArrow.transform);
        _currentRightArrowRotation += deltaRotation;
        _currentRightArrowRotation = Mathf.Clamp(_currentRightArrowRotation, MinRotation, MaxRotation);
        rightArrow.transform.DOLocalRotate(new Vector3(0, 0, _currentRightArrowRotation), 1.0f);
    }

    public static class GeneratorSwitchEvents
    {
        public static Action CheckArrows;
        public static Action<float> AdjustArrowRotationLeft;
        public static Action<float> AdjustArrowRotationRight;
    }

    private void ResetGenerator()
    {
        _currentLeftArrowRotation = 0f;
        leftArrow.transform.DOLocalRotate(new Vector3(0, 0, _currentLeftArrowRotation), 0.5f);
        
        _currentRightArrowRotation = 0f;
        rightArrow.transform.DOLocalRotate(new Vector3(0, 0, _currentRightArrowRotation), 0.5f);
        
        ResetSwitches();
    }
    
    private void OpenBox()
    {
        fuseBoxDoor.transform.DOLocalRotate(new Vector3(0, 125, 0), 1.0f, RotateMode.FastBeyond360);
        SoundManager.PlaySound3D(fuseBoxSound,fuseBoxDoor.transform);
        _fuseCollider.enabled = true;
        _fuseRigibody.isKinematic = false;
    }

    private void ResetSwitches()
    {
        foreach (var generatorSwitch in generatorSwitches)
        {
            generatorSwitch.ResetSwitchPositionToMinHeight();
        }
    }
}
