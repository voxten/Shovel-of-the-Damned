using UnityEngine;
using System;
using StarterAssets;
using UnityEngine.UI;
using DG.Tweening;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private Image interactionIcon;

    [Header("Interactable Objects")]
    [SerializeField] private float interactionRange;
    [SerializeField] private LayerMask interactionLayerMask;
    
    [Header("Moveable Objects")]
    [SerializeField] private LayerMask moveableLayerMask;
    private bool _moveableState;

    private Camera _playerCamera;

    private InteractableObject _currentInteractable;
    private PuzzleInteraction _puzzleInteraction;

    private bool _isInteractingPuzzle;
    private bool _isInRange;

    private void Awake()
    {
        _playerCamera = Camera.main;
    }
    
    private void OnEnable()
    {
        InteractionEvents.GetInteraction += GetInteraction;
        InteractionEvents.SetInteractionView += SetInteractionView;
        InteractionEvents.TogglePuzzleCollider += TogglePuzzleCollider;
        InteractionEvents.DisableInteractionIcon += DisableInteractionIcon;
    }

    private void OnDisable()
    {
        InteractionEvents.GetInteraction -= GetInteraction;
        InteractionEvents.SetInteractionView -= SetInteractionView;
        InteractionEvents.TogglePuzzleCollider -= TogglePuzzleCollider;
        InteractionEvents.DisableInteractionIcon -= DisableInteractionIcon;
    }

    private void Update()
    {
        HandleRaycastInteraction();
    }

    private void HandleRaycastInteraction()
    {
        var ray = _playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        
        if (Physics.Raycast(ray, out var hit, interactionRange, interactionLayerMask))
        {
            var interactable = hit.collider.GetComponent<InteractableObject>();
            
            if (interactable != null)
            {
                if (_currentInteractable != interactable)
                {
                    _currentInteractable = interactable;
                    if (_currentInteractable is PuzzleInteraction puzzleInteraction)
                    {
                        if (!puzzleInteraction.puzzleObject.isFinished)
                        {
                            _isInRange = true;
                            EnableInteractionIcon();
                        }
                        
                    }
                    else if (_currentInteractable is InputInteraction inputInteraction)
                    {
                        if (!inputInteraction.interactableInputObject.isFinished)
                        {
                            if (ToolManager.Instance.CurrentTool is not null)
                            {
                                EnableInteractionIcon();
                            }
                            else
                            {
                                DisableInteractionIcon();
                            }
                        }
                    }
                    else if (_currentInteractable is NewCableEndPoint cableEndPoint)
                    {
                        if (cableEndPoint.GetCable() == null)
                        {
                            DisableInteractionIcon();
                        }
                        else
                        {
                            EnableInteractionIcon();
                        }
                    }
                    else
                    {
                        EnableInteractionIcon();
                    }
                }
            }
            else
            {
                DisableInteractionIcon();
            }
        }

        if (DragObject.DragEvents.GetDragging())
        {
            DisableInteractionIcon();
        }
        else
        {
            if (Physics.Raycast(ray, out hit, interactionRange, moveableLayerMask))
            {
                _moveableState = true;
                EnableInteractionIcon();
            }
            else
            {
                DisableInteractionIcon();
            }
        }
        
        if (Input.GetMouseButtonDown(0) && !_isInteractingPuzzle)
        {
            TryInteract();
        }

        if (Input.GetMouseButtonDown(1) && _isInteractingPuzzle && !_puzzleInteraction.puzzleObject.isFinished)
        {
            _puzzleInteraction.EndInteract();
            TogglePuzzleCollider();
            SetInteractionView(false);
            DisableInteractionIcon();
            _puzzleInteraction = null;
        }
    }
    
    private void TryInteract()
    {
        if (_isInRange && _currentInteractable != null)
        {
            if (_currentInteractable.Interact())
            {
                if (_currentInteractable is PuzzleInteraction puzzleInteraction)
                {
                    if (puzzleInteraction.puzzleObject is KeyPanelPuzzle)
                    {
                        if (FusePuzzle.FuseEvents.GetFinished())
                        {
                            _puzzleInteraction = puzzleInteraction;
                            if (_puzzleInteraction.puzzleObject.isFinished) return;
                            TogglePuzzleCollider();
                            SetInteractionView(true);
                        }
                        else
                        {
                            Narration.DisplayText?.Invoke("There's no power...");
                        }
                    }
                    else
                    {
                        _puzzleInteraction = puzzleInteraction;
                        if (_puzzleInteraction.puzzleObject.isFinished) return;
                        TogglePuzzleCollider();
                        SetInteractionView(true);
                    }
                }
            }
        }
    }

    private void TogglePuzzleCollider()
    {
        _puzzleInteraction.puzzleCollider.enabled = !_puzzleInteraction.puzzleCollider.enabled;
        FirstPersonController.PlayerEvents.ToggleCapsule();
        FirstPersonController.PlayerEvents.ToggleController();
    }

    private void SetInteractionView(bool state)
    {
        _isInteractingPuzzle = state;
        Utilitis.SetCursorState(!state);
        SwitchCamera(state);
        ToolManager.Instance.ToggleTool();
    }
    
    private void SwitchCamera(bool state)
    {
        if (!state)
        {
            CameraSwitch.CameraEvents.SwitchToDefault();
        }
        else
        {
            _puzzleInteraction = _currentInteractable as PuzzleInteraction;
            if (_puzzleInteraction == null)
            {
                return;
            }
            CameraSwitch.CameraEvents.SwitchCamera(_puzzleInteraction.virtualCamera);
        }
        FirstPersonController.PlayerEvents.ToggleMove();
    }

    private void DisableInteractionIcon()
    {
        if (_currentInteractable != null)
        {
            _isInRange = false;
            _currentInteractable = null;
        }

        if (_moveableState)
        {
            _moveableState = false;
        }
        
        interactionIcon.DOFade(0f, 0.5f);
    }
    
    private void EnableInteractionIcon()
    {
        _isInRange = true;
        interactionIcon.DOFade(1f, 0.5f);
    }
    
    private bool GetInteraction()
    {
        return _isInteractingPuzzle;
    }
    
    public static class InteractionEvents 
    {
        public static Func<bool> GetInteraction;
        public static Action<bool> SetInteractionView;
        public static Action TogglePuzzleCollider;
        public static Action DisableInteractionIcon;
    }
}
