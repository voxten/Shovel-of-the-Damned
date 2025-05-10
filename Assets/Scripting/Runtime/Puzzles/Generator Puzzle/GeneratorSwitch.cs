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
            
            transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, targetRotation), 0.5f);
            
            SoundManager.PlaySound3D(Sound.SwitchOn, transform);
        }
    }

    public bool GetIsSwitched()
    {
        return _isSwitched;
    }
}