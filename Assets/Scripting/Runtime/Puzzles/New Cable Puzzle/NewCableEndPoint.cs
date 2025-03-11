using System.Collections;
using UnityEngine;

public class NewCableEndPoint : InteractableObject
{
    [SerializeField] private Sound unhookCable;
    public int cableNumber;
    private NewCable _cable;
    private HingeJoint _hingeJoint;
    public bool isWaiting;

    private void Awake()
    {
        _hingeJoint = GetComponent<HingeJoint>();
    }

    public override bool Interact()
    {
        if (_cable != null && !CheckCablesButton.CablePuzzle.GetFinished())
        {
            StartCoroutine(UnhookCable());
        }
        return true;
    }
    
    private void ResetCable()
    {
        _hingeJoint.connectedBody = null;
        _cable.endPointNumber = 0;
        _cable = null;
    }
    
    public void SetCableEndPoint(NewCable newCable)
    {
        _hingeJoint.connectedBody = newCable.cableRigidbody;
        _cable = newCable;
    }

    public NewCable GetCable()
    {
        return _cable != null ? _cable : null;
    }

    private IEnumerator UnhookCable()
    {
        isWaiting = true;
        SoundManager.PlaySound3D(unhookCable, transform);
        ResetCable();
        yield return new WaitForSeconds(1);
        isWaiting = false;
    }
}
