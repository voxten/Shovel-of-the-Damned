using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ElevatorButton : InteractableObject
{
    [SerializeField] private GameObject elevatorObject;
    [SerializeField] private GeneratorPuzzle generatorPuzzle;
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private ElevatorTrigger elevatorTrigger;

    [SerializeField] private float elevatorSpeed = 5f;
    
    [SerializeField] private List<GameObject> elevatorColliders;
    
    private float _elevatorMinY = -2.88f;
    private float _elevatorMaxY = 0f;

    private bool _doesElevatorWork;
    private bool _isAtMax;
    private bool _isMoving;

    public override bool Interact()
    {
        if (_isMoving || !elevatorTrigger.GetIsTriggered()) 
            return false;

        if (generatorPuzzle.isFinished)
        {
            _doesElevatorWork = true;
            AnimateButton();
            AnimateElevator();
        }
        else
        {
            AnimateButton();
            Narration.DisplayText?.Invoke("There is no power...");
        }

        return false;
    }

    private void AnimateButton()
    {
        SoundManager.PlaySound3D(Sound.ButtonClick, buttonObject.transform);
        buttonObject.transform.DOLocalMoveY(-0.5f, 0.3f)
            .OnComplete(() =>
            {
                buttonObject.transform.DOLocalMoveY(0f, 0.3f);
            });
    }

    private void AnimateElevator()
    {
        if (!_doesElevatorWork) return;

        foreach (var eleCollider in elevatorColliders)
        {
            eleCollider.SetActive(true);
        }
        SoundManager.PlaySound3D(Sound.ElevatorLift, elevatorTrigger.gameObject.transform);

        _isMoving = true;

        float targetY = _isAtMax ? _elevatorMinY : _elevatorMaxY;
        _isAtMax = !_isAtMax;

        Vector3 targetPosition = new Vector3(
            elevatorObject.transform.localPosition.x,
            targetY,
            elevatorObject.transform.localPosition.z
        );

        elevatorObject.transform.DOLocalMove(targetPosition, elevatorSpeed)
            .OnComplete(() =>
            {
                _isMoving = false;
                foreach (var eleCollider in elevatorColliders)
                {
                    eleCollider.SetActive(false);
                }
            });
    }
}