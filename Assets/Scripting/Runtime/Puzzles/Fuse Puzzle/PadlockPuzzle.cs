using UnityEngine;
using DG.Tweening;

public class PadlockPuzzle : InteractableInputObject
{
    [SerializeField] private Sound cutPadlockSound;
    [SerializeField] private Collider fuseBoxCollider;
    [SerializeField] private Collider handlerCollider;
    [SerializeField] private GameObject fuseBoxDoor;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    protected override void EndPuzzle()
    {
        base.EndPuzzle();
        _collider.enabled = false;
        fuseBoxCollider.enabled = true;
        fuseBoxDoor.transform.DOLocalRotate(new Vector3(0, 125, 0), 1.0f, RotateMode.FastBeyond360);
    }

    public override void InteractPuzzle()
    {
        EndPuzzle();
        handlerCollider.enabled = false;
        SoundManager.PlaySound3D(cutPadlockSound, handlerCollider.gameObject.transform);
    }
}