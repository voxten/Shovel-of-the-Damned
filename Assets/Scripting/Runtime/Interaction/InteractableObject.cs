using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public float iconHeight;
    public abstract bool Interact();

    public virtual void EndInteract() {
        
    }
}