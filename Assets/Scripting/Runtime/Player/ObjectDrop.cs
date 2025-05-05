using System;
using UnityEngine;

public class ObjectDrop : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            SoundManager.PlaySound3D(Sound.Drop, transform);
        }
    }
}
