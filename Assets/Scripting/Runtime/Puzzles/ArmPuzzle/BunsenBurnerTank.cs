using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BunsenBurnerTank : InteractableObject
{
    [SerializeField] private GameObject burner;
    [SerializeField] private GameObject flameParticles;
    private Collider _burnerCollider;
    private bool _isFirst = true;
    public bool isOn;

    private void Awake()
    {
        _burnerCollider = burner.GetComponent<Collider>();
    }

    private void Start()
    {
        flameParticles.SetActive(false);
    }

    public override bool Interact()
    {
        if (_isFirst)
        {
            isOn = true;
            flameParticles.SetActive(true);
            _burnerCollider.enabled = true;
            _isFirst = false;
        }
        return true;
    }
}
