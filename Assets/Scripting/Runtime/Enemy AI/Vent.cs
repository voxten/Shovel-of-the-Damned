using UnityEngine;
using System.Collections.Generic;

public class Vent : MonoBehaviour
{
    [Header("Gizmo Settings")]
    [SerializeField] private Color ventColor = new(0, 0.5f, 1f, 0.5f);
    [SerializeField] private float ventRadius = 0.5f;
    [SerializeField] private Color pointsColor = Color.yellow;
    [SerializeField] private float pointSize = 0.6f;
    
    [Header("Points Settings")]
    [SerializeField] private List<GameObject> points = new();

    public List<GameObject> Points => points;

    private void OnDrawGizmos()
    {
        Gizmos.color = ventColor;
        Gizmos.DrawSphere(transform.position, ventRadius);
        
        // Draw points and connections
        Gizmos.color = pointsColor;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] == null) continue;
            
            Gizmos.DrawSphere(points[i].transform.position, pointSize);
            
            // Draw line to next point
            if (i < points.Count - 1 && points[i+1] != null)
            {
                Gizmos.DrawLine(points[i].transform.position, points[i+1].transform.position);
            }
        }
    }
}