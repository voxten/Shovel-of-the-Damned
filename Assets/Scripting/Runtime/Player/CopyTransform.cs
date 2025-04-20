using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    public Transform sourceObject; // Obiekt, z kt�rego kopiujemy transform
    public Transform targetObject; // Obiekt, do kt�rego kopiujemy transform

    [Header("Ustawienia p�ynno�ci")]
    [Range(0f, 1f)]
    public float positionSmoothTime = 0.1f; // Czas p�ynnego przej�cia pozycji (0 - brak, 1 - bardzo p�ynne)
    [Range(0f, 1f)]
    public float rotationSmoothTime = 0.1f; // Czas p�ynnego przej�cia rotacji (0 - brak, 1 - bardzo p�ynne)

    void LateUpdate()
    {
        if (sourceObject != null && targetObject != null)
        {
            // P�ynne kopiowanie pozycji
            targetObject.position = Vector3.Lerp(targetObject.position, sourceObject.position, positionSmoothTime);

            // P�ynne kopiowanie rotacji
            targetObject.rotation = Quaternion.Slerp(targetObject.rotation, sourceObject.rotation, rotationSmoothTime);

            // P�ynne kopiowanie skali (je�li wymagane)
            targetObject.localScale = Vector3.Lerp(targetObject.localScale, sourceObject.localScale, positionSmoothTime);
        }
    }
}
