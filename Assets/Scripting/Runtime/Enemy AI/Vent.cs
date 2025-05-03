using UnityEngine;

public class Vent : MonoBehaviour
{
    [Header("Gizmo Settings")]
    [SerializeField] private Color ventColor = new Color(0, 0.5f, 1f, 0.5f);
    [SerializeField] private float ventRadius = 0.5f;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = ventColor;
        Gizmos.DrawSphere(transform.position, ventRadius);
    }
}