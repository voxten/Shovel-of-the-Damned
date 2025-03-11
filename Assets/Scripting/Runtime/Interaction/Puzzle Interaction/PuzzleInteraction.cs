using Unity.Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class PuzzleInteraction : InteractableObject
{
    [Header("Puzzle Object")]
    public PuzzleObject puzzleObject;
    public Collider puzzleCollider;
    
    [Header("Virtual Camera")]
    public CinemachineCamera virtualCamera;

    [Header("Tool")]
    [SerializeField] private bool _requireTool;
    [SerializeField, ShowIf("_requireTool"), Required] private ToolType _requiredToolType;
    
    private void Awake()
    {
        puzzleCollider = GetComponent<Collider>();
    }

    public override bool Interact()
    {
        if (!_requireTool)
        {
            // if tool is not required, open a puzzle
            puzzleObject.OpenPuzzle();
            return true;
        }
        /*
        
        if (ToolManager.Instance.CurrentTool is null) 
        {
            Narration.DisplayText?.Invoke("I need some kind of tool to do this...");
            return false;
        }

        if (!ToolManager.Instance.CompareToolType(_requiredToolType))
        {
            Narration.DisplayText?.Invoke("It's not gonna work...");
            return false;
        }*/
        
        puzzleObject.OpenPuzzle();
        return true;
    }

    public override void EndInteract() {
        puzzleObject.QuitPuzzle();
    }
}
