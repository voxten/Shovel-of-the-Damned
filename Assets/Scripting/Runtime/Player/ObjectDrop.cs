using System;
using UnityEngine;

public class ObjectDrop : MonoBehaviour
{
    [SerializeField] private Sound dropSound;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            SoundManager.PlaySound3D(dropSound, transform);
        }
    }
}
