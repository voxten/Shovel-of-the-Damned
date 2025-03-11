using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class InputInteraction : InteractableObject
{
    public InteractableInputObject interactableInputObject;
    
    public bool customInteraction;
    [SerializeField] private bool _requireTool;
    [SerializeField, ShowIf("_requireTool"), Required] private ToolType _requiredToolType;
    [SerializeField] private string customMessage;
    
    public override bool Interact()
    {
        if (!_requireTool) return true;

        if (ToolManager.Instance.CurrentTool is null) 
        {
            Debug.Log("Dua1");
            if (customMessage != String.Empty)
            {
                Narration.DisplayText?.Invoke(customMessage);
            }
            else
            {
                Narration.DisplayText?.Invoke("I need some kind of tool to do this...");
            }
            return false;
        }

        if (!ToolManager.Instance.CompareToolType(_requiredToolType))
        {
            Narration.DisplayText?.Invoke("It's not gonna work...");
            return false;
        }
        
        if (customInteraction) 
        {
            Debug.Log("Dua2");
            interactableInputObject.InteractPuzzle();
            return true;
        }
        
        //ToolManager.Instance.RemoveTool();
        interactableInputObject.InteractPuzzle();
        return true;
    }
}
