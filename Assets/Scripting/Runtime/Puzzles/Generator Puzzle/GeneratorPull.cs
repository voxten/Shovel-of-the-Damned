using System;
using System.Collections;
using UnityEngine;

public class GeneratorPull : InteractableObject
{
    private bool _canPull;
    private Animator _animator;
    private Collider _collider;

    private void Awake()
    {
        _canPull = true;
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
    }

    public override bool Interact()
    {
        if (_canPull)
        {
            _canPull = false;
            StartCoroutine(StartPull());
        }
        return true;
    }

    private IEnumerator StartPull()
    {
        _animator.SetTrigger("StartPull");
        SoundManager.PlaySound3D(Sound.GeneratorPull, transform);
        yield return new WaitForSeconds(1.5f);
        CheckPull();
    }

    private void CheckPull()
    {
        if (GeneratorPuzzle.GeneratorEvents.CheckPuzzle())
        {
            _collider.enabled = false;
            _canPull = false;
        }
        else
        {
            _canPull = true;
        }
    }
}
