using System;
using UnityEngine;

public class Vent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Vent" + gameObject.name + " has entered");
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Vent" + gameObject.name + " has entered");
            other.GetComponent<EnemyAI>().CheckVent(this);
        }
    }
}
