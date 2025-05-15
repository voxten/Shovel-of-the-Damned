using UnityEngine;
using System;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private Image interactionIcon;

    [Header("Interactable Objects")]
    [SerializeField] private float interactionRange;
    [SerializeField] private LayerMask interactionLayerMask;
    
    [Header("Moveable Objects")]
    [SerializeField] private LayerMask moveableLayerMask;
    
    [Header("Door/Drawer Objects")]
    [SerializeField] private LayerMask doorLayerMask;

    [Header("Light Object")] 
    [SerializeField] private GameObject coneObject;

    private Camera _playerCamera;

    private InteractableObject _currentInteractable;
    private PuzzleInteraction _puzzleInteraction;

    private bool _isInteractingPuzzle;
    private bool _isInRange;

    private GameObject _lastHighlightedObject;
    private Outline _currentOutline;

    private void Awake()
    {
        _playerCamera = Camera.main;
        interactionIcon.DOFade(0f, 0f);
    }
    
    private void OnEnable()
    {
        InteractionEvents.ExitPuzzleInteraction += ExitPuzzleInteraction;
        InteractionEvents.SetInteractionView += SetInteractionView;
        InteractionEvents.TogglePuzzleCollider += TogglePuzzleCollider;
        InteractionEvents.DisableInteractionIcon += DisableInteractionIcon;
        DragObject.DragEvents.ObjectDropped += OnObjectDropped;
    }

    private void OnDisable()
    {
        InteractionEvents.ExitPuzzleInteraction -= ExitPuzzleInteraction;
        InteractionEvents.SetInteractionView -= SetInteractionView;
        InteractionEvents.TogglePuzzleCollider -= TogglePuzzleCollider;
        InteractionEvents.DisableInteractionIcon -= DisableInteractionIcon;
        DragObject.DragEvents.ObjectDropped -= OnObjectDropped;
    }

    private void Update()
    {
        HandleRaycastInteraction();
    }

    private void HandleRaycastInteraction()
    {
        var ray = _playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        GameObject objectToHighlight = null;
        bool shouldShowIcon = false;

        // Check interaction layer first
        if (Physics.Raycast(ray, out var hit, interactionRange, interactionLayerMask))
        {
            var interactable = hit.collider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                _currentInteractable = interactable;
                objectToHighlight = interactable.gameObject;

                if (_currentInteractable is PuzzleInteraction puzzleInteraction)
                {
                    if (!puzzleInteraction.puzzleObject.isFinished)
                    {
                        _isInRange = true;
                        shouldShowIcon = true;
                    }
                }
                else if (_currentInteractable is NewCableEndPoint cableEndPoint)
                {
                    shouldShowIcon = cableEndPoint.GetCable() != null;
                }
                else
                {
                    shouldShowIcon = true;
                }
            }
        }
        // Check moveable objects if no interactable was found
        else if (Physics.Raycast(ray, out hit, interactionRange, moveableLayerMask) && !DragObject.DragEvents.GetDragging())
        {
            objectToHighlight = hit.collider.gameObject;
            shouldShowIcon = true;
        }
        // Check door objects if no other objects were found
        else if (Physics.Raycast(ray, out hit, interactionRange, doorLayerMask) && !DragObject.DragEvents.GetDragging())
        {
            objectToHighlight = hit.collider.gameObject;
            shouldShowIcon = true;
        }
        else
        {
            _currentInteractable = null;
        }

        // Update highlight only if needed
        if (objectToHighlight != _lastHighlightedObject)
        {
            RemoveHighlight();
            if (objectToHighlight != null)
            {
                HighlightObject(objectToHighlight);
            }
            _lastHighlightedObject = objectToHighlight;
        }

        // Update interaction icon
        if (shouldShowIcon)
        {
            EnableInteractionIcon();
        }
        else
        {
            DisableInteractionIcon();
        }

        // Handle interaction input
        if (Input.GetMouseButtonDown(0) && !_isInteractingPuzzle)
        {
            TryInteract();
        }

        if (Input.GetMouseButtonDown(1) && _isInteractingPuzzle && _puzzleInteraction != null && !_puzzleInteraction.puzzleObject.isFinished)
        {
            ExitPuzzleInteraction();
        }
    }

    private void ExitPuzzleInteraction()
    {
        _puzzleInteraction.EndInteract();
        TogglePuzzleCollider();
        SetInteractionView(false);
        coneObject.SetActive(true);
        DisableInteractionIcon();
        _puzzleInteraction = null;
    }
    
    private void TryInteract()
    {
        if (_isInRange && _currentInteractable != null && Time.timeScale > 0)
        {
            if (_currentInteractable.Interact())
            {
                if (_currentInteractable is PuzzleInteraction puzzleInteraction)
                {
                    _puzzleInteraction = puzzleInteraction;
                    if (_puzzleInteraction.puzzleObject.isFinished) return;
                    TogglePuzzleCollider();
                    coneObject.SetActive(false);
                    SetInteractionView(true);
                }
            }
        }
    }

    private void TogglePuzzleCollider()
    {
        _puzzleInteraction.puzzleCollider.enabled = !_puzzleInteraction.puzzleCollider.enabled;
        FirstPersonController.PlayerEvents.TogglePlayerModel();
        ToggleIcon();
        FirstPersonController.PlayerEvents.ToggleController();
    }

    private void SetInteractionView(bool state)
    {
        _isInteractingPuzzle = state;
        Utilitis.SetCursorState(!state);
        SwitchCamera(state);
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
    
    private void EnableInteractionIcon()
    {
        _isInRange = true;
        interactionIcon.DOFade(1f, 0.5f);
    }

    private void DisableInteractionIcon()
    {
        _isInRange = false;
        interactionIcon.DOFade(0f, 0.5f);
    }
    
    private void ToggleIcon()
    {
        interactionIcon.gameObject.SetActive(!interactionIcon.gameObject.activeSelf);
    }
    
    private bool GetInteraction()
    {
        return _isInteractingPuzzle;
    }
    
    public static class InteractionEvents 
    {
        public static Action ExitPuzzleInteraction;
        public static Action<bool> SetInteractionView;
        public static Action TogglePuzzleCollider;
        public static Action DisableInteractionIcon;
    }

    private void HighlightObject(GameObject obj)
    {
        if (obj == null) return;

        // Check if the object already has an outline
        _currentOutline = obj.GetComponent<Outline>();
        if (_currentOutline == null)
        {
            _currentOutline = obj.AddComponent<Outline>();
        }

        _currentOutline.OutlineMode = Outline.Mode.OutlineVisible;
        _currentOutline.OutlineColor = Color.white;
        _currentOutline.OutlineWidth = 2f;
        _currentOutline.enabled = true;
    }
    
    private void RemoveHighlight()
    {
        if (_currentOutline != null)
        {
            // Only disable the outline instead of destroying it
            _currentOutline.enabled = false;
            _currentOutline = null;
        }
        _lastHighlightedObject = null;
    }

    private void OnObjectDropped()
    {
        RemoveHighlight();
    }
}