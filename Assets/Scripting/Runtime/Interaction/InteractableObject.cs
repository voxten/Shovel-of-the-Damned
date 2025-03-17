using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public virtual bool Interact()
    {
        InteractionSystem.InteractionEvents.DisableInteractionIcon();
        return true;
    }

    public virtual void EndInteract() {
        
    }
}