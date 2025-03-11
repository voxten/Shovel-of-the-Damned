using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class NewCable : MonoBehaviour
{
    [SerializeField] private Light cableLight;
    [SerializeField] private Sound controlSound;
    [SerializeField] private Sound cableInsert;
    public Rigidbody cableRigidbody;
    
    public int endPointNumber;
    public int correctPointNumber;

    private NewCableEndPoint _endPoint;

    private void Awake()
    {
        cableRigidbody = GetComponent<Rigidbody>();
        endPointNumber = 0;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CableEndPoint"))
        {
            if (_endPoint == null)
            {
                SetCable(other);
            }
            
            if (!_endPoint.isWaiting)
            {
                SetCable(other);
            }
        }
    }

    private void SetCable(Collider other)
    {
        SoundManager.PlaySound3D(cableInsert, transform);
        _endPoint = other.GetComponent<NewCableEndPoint>();
        endPointNumber = _endPoint.cableNumber;
        transform.position = other.transform.position;
        _endPoint.SetCableEndPoint(this);
    }

    public bool CheckCable()
    {
        return endPointNumber == correctPointNumber;
    }

    public void SetLight()
    {
        StartCoroutine(WaitForLight());
    }

    private IEnumerator WaitForLight()
    {
        cableLight.enabled = true;
        SoundManager.PlaySound3D(controlSound,cableLight.transform);
        yield return new WaitForSeconds(1);
        if (CheckCablesButton.CablePuzzle.GetFinished()) yield break;
        SoundManager.PlaySound3D(controlSound,cableLight.transform);
        cableLight.enabled = false;
    }
}
