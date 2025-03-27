using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CustomObjectPoint : MonoBehaviour
{
    public Transform customPoint; // Assign a custom point through the inspector

    private void OnDrawGizmos()
    {
        if (customPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(customPoint.position, 0.1f);
            Gizmos.DrawLine(transform.position, customPoint.position);
        }
    }

    public Vector3 GetCustomPoint()
    {
        return customPoint != null ? customPoint.position : transform.position;
    }
}