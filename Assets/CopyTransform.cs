using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    public Transform sourceObject; // Obiekt, z którego kopiujemy transform
    public Transform targetObject; // Obiekt, do którego kopiujemy transform

    [Header("Ustawienia p³ynnoœci")]
    [Range(0f, 1f)]
    public float positionSmoothTime = 0.1f; // Czas p³ynnego przejœcia pozycji (0 - brak, 1 - bardzo p³ynne)
    [Range(0f, 1f)]
    public float rotationSmoothTime = 0.1f; // Czas p³ynnego przejœcia rotacji (0 - brak, 1 - bardzo p³ynne)

    void LateUpdate()
    {
        if (sourceObject != null && targetObject != null)
        {
            // P³ynne kopiowanie pozycji
            targetObject.position = Vector3.Lerp(targetObject.position, sourceObject.position, positionSmoothTime);

            // P³ynne kopiowanie rotacji
            targetObject.rotation = Quaternion.Slerp(targetObject.rotation, sourceObject.rotation, rotationSmoothTime);

            // P³ynne kopiowanie skali (jeœli wymagane)
            targetObject.localScale = Vector3.Lerp(targetObject.localScale, sourceObject.localScale, positionSmoothTime);
        }
    }
}
