using UnityEngine;

public class ToolSway : MonoBehaviour
{ 
    [SerializeField] private float _swayAmount = 0.02f;
    [SerializeField] private float _swayRotationMultiplier = 100f;
    [SerializeField] private float _swaySpeed = 6f;

    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.localPosition;
    }

    private void Update()
    {
        float mouseInputX = -Input.GetAxis("Mouse X") * _swayAmount;
        float mouseInputY = -Input.GetAxis("Mouse Y") * _swayAmount;

        Vector3 targetPosition = new Vector3(mouseInputX, mouseInputY, 0);
        
        Quaternion rotationX = Quaternion.AngleAxis(-mouseInputY * _swayRotationMultiplier, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseInputX * _swayRotationMultiplier, Vector3.up);
        
        Quaternion targetRotation = rotationX * rotationY;
        
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition + _initialPosition, Time.deltaTime * _swaySpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * _swaySpeed);
    }
}
