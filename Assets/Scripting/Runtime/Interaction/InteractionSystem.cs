using UnityEngine;
using System;
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

    private GameObject _currentlyHighlighted;
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

        if (Physics.Raycast(ray, out var hit, interactionRange, interactionLayerMask))
        {
            var interactable = hit.collider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                if (_currentInteractable != interactable)
                {
                    _currentInteractable = interactable;
                    _currentlyHighlighted = interactable.gameObject;
                    HighlightObject(_currentlyHighlighted);

                    if (_currentInteractable is PuzzleInteraction puzzleInteraction)
                    {
                        if (!puzzleInteraction.puzzleObject.isFinished)
                        {
                            _isInRange = true;
                            EnableInteractionIcon();
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
                RemoveHighlight();
                DisableInteractionIcon();
            }
        }
        else if (Physics.Raycast(ray, out hit, interactionRange, moveableLayerMask) && !DragObject.DragEvents.GetDragging())
        {
            if (_currentlyHighlighted != hit.collider.gameObject)
            {
                RemoveHighlight();
                _currentlyHighlighted = hit.collider.gameObject;
                HighlightObject(_currentlyHighlighted);
            }
            EnableInteractionIcon();
        }
        else if (Physics.Raycast(ray, out hit, interactionRange, doorLayerMask) && !DragObject.DragEvents.GetDragging())
        {
            if (_currentlyHighlighted != hit.collider.gameObject)
            {
                RemoveHighlight();
                _currentlyHighlighted = hit.collider.gameObject;
                HighlightObject(_currentlyHighlighted);
            }
            EnableInteractionIcon();
        }
        else
        {
            RemoveHighlight();
            DisableInteractionIcon();
        }

        // Interaction (klikni�cie)
        if (Input.GetMouseButtonDown(0) && !_isInteractingPuzzle)
        {
            TryInteract();
        }

        // Exit from puzzle (prawy przycisk)
        if (Input.GetMouseButtonDown(1) && _isInteractingPuzzle && !_puzzleInteraction.puzzleObject.isFinished)
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

        if (_currentInteractable != null)
        {
            Outline outline = _currentInteractable.GetComponent<Outline>();
            if (outline == null)
            {
                outline = _currentInteractable.gameObject.AddComponent<Outline>();
            }
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 2f;
        }
    }

    private void DisableInteractionIcon()
    {
        _isInRange = false;
        if (_currentInteractable != null)
        {
            Outline outline = _currentInteractable.GetComponent<Outline>();
            if (outline != null)
            {
                Destroy(outline); // Remove outline when interaction ends
            }
        }
        _currentInteractable = null;
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

        Outline outline = obj.GetComponent<Outline>();
        if (outline == null)
        {
            outline = obj.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 2f;
        }
        _currentOutline = outline;
    }

    private void RemoveHighlight()
    {
        // Je�li co� jest trzymane (dragowane) - NIE usuwamy outline!
        if (DragObject.DragEvents.GetDragging())
            return;

        if (_currentOutline != null)
        {
            Destroy(_currentOutline);
            _currentOutline = null;
            _currentlyHighlighted = null;
        }
    }

    private void OnObjectDropped()
    {
        _currentlyHighlighted = null;
        _currentOutline = null;
    }



}
