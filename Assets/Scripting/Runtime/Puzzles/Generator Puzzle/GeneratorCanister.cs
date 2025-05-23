using System;
using UnityEngine;

public class GeneratorCanister : MonoBehaviour
{
    [SerializeField] private Material normalMaterial;
    private BoxCollider _boxCollider;
    private Rigidbody _rigidbody;
    private MeshRenderer _meshRenderer;
    private Animator _animator;
    
    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        GeneratorPuzzle generatorPuzzle = FindObjectOfType<GeneratorPuzzle>();
        if (generatorPuzzle.isFinished && generatorPuzzle.isAfterLoad)
        {
            _meshRenderer.materials = new[] { normalMaterial };
            _meshRenderer.material = normalMaterial;
            ResetCanister();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Canister"))
        {
            _meshRenderer.materials = new[] { normalMaterial };
            _meshRenderer.material = normalMaterial;
            SoundManager.PlaySound3D(Sound.CanisterPour, transform);
            _animator.SetTrigger("Pour");
            DragObject.DragEvents.DropObject();
            Destroy(other.gameObject);
        }
    }

    public void ResetCanister()
    {
        gameObject.layer = LayerMask.NameToLayer("Moveable");
        GeneratorPuzzle.GeneratorEvents.SetFuelLed();
        _boxCollider.isTrigger = false;
        _rigidbody.isKinematic = false;
        _animator.enabled = false;
    }
}