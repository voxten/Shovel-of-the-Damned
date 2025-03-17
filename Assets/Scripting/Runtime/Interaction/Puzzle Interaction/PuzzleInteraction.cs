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
    [SerializeField] private bool requireItem;
    [SerializeField, ShowIf("requireItem"), Required] private Item requiredItem;
    
    private void Awake()
    {
        puzzleCollider = GetComponent<Collider>();
    }

    public override bool Interact()
    {
        if (!requiredItem)
        {
            // if item is not required, open a puzzle
            puzzleObject.OpenPuzzle();
            return true;
        }

        if (!Inventory.InventoryEvents.FindItem(requiredItem))
        {
            Narration.DisplayText?.Invoke("I don't have required item...");
            return false;
        }
        
        puzzleObject.OpenPuzzle();
        return true;
    }

    public override void EndInteract() 
    {
        puzzleObject.QuitPuzzle();
    }
}
