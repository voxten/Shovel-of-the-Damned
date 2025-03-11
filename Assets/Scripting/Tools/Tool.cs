using UnityEngine;

public class Tool : InteractableObject
{
    public ToolType toolType; 
    public override bool Interact()
    {
        // Debug.Log($"Interacting with Tool {gameObject.name}");
        ToolManager.Instance.TryPickupTool(this);
        return true;
    }

    public void EnableColliders()
    {
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = true;
        }
    }
    
    public void DisableColliders()
    {
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }
}

public enum ToolType
{
    Fuse,
    PipeWrench,
    CablePliers,
    Welder
}