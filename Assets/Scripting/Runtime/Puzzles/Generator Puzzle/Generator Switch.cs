using UnityEngine;
using DG.Tweening;

public class GeneratorSwitch : MonoBehaviour
{
    [SerializeField] private GameObject switchObject;

    [Header("Arrow Rotations")]
    [SerializeField] private float leftArrowRotation;
    [SerializeField] private float rightArrowRotation;

    [Header("Rotate Arrows?")] 
    [SerializeField] private bool rotateLeftArrow;
    [SerializeField] private bool rotateRightArrow;

    [Header("Audio")] 
    [SerializeField] private Sound switchOnSound;
    [SerializeField] private Sound switchOffSound;

    private const float MinHeight = 0.05f;
    private const float MaxHeight = -0.05f;
    private bool _currentHeight;

    private void Awake()
    {
        _currentHeight = false;
    }
    
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_currentHeight)
            {
                SoundManager.PlaySound3D(switchOnSound, transform);
                switchObject.transform.DOLocalMoveY(MaxHeight, 0.25f).SetEase(Ease.InOutSine);
                MoveArrows();
            }
            else
            {
                SoundManager.PlaySound3D(switchOffSound, transform);
                switchObject.transform.DOLocalMoveY(MinHeight, 0.25f).SetEase(Ease.InOutSine);
                MoveArrows();
            }
            _currentHeight = !_currentHeight;
        }
    }

    private void MoveArrows()
    {
        if (rotateLeftArrow)
        {
            Generator.GeneratorSwitchEvents.AdjustArrowRotationLeft(_currentHeight ? leftArrowRotation : -leftArrowRotation);
        }

        if (rotateRightArrow)
        {
            Generator.GeneratorSwitchEvents.AdjustArrowRotationRight(_currentHeight ? rightArrowRotation : -rightArrowRotation);
        }

        Generator.GeneratorSwitchEvents.CheckArrows();
    }
    
    public void ResetSwitchPositionToMinHeight()
    {
        _currentHeight = false;
    
        // Animate the switch to MinHeight
        switchObject.transform.DOLocalMoveY(MaxHeight, 0.25f).SetEase(Ease.InOutSine);
    }


}