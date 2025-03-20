using UnityEngine;
using DG.Tweening;

public class GeneratorSwitch : MonoBehaviour
{
    [SerializeField] private float rotationZ1;
    [SerializeField] private float rotationZ2;
    [SerializeField] private Light[] lights;
    private bool _isSwitched;

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var targetRotation = _isSwitched ? rotationZ1 : rotationZ2;
            
            lights[0].enabled = _isSwitched;
            lights[1].enabled = !_isSwitched;
            _isSwitched = !_isSwitched;
            
            var currentX = transform.eulerAngles.x;
            var currentY = transform.eulerAngles.y;
            
            transform.DOLocalRotate(new Vector3(currentX, currentY, targetRotation), 0.5f, RotateMode.Fast);
        }
    }

    public bool GetIsSwitched()
    {
        return _isSwitched;
    }
}