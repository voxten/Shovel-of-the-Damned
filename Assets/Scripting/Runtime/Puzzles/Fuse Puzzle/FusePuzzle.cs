using System;
using UnityEngine;

public class FusePuzzle : InteractableInputObject
{
    [SerializeField] private GameObject[] fuses;
    [SerializeField] private Sound fuseInputSound;
    [SerializeField] private int count;

    private int _currentCount;
    private Collider _collider;

    private void OnEnable()
    {
        FuseEvents.GetFinished += GetFinished;
    }

    private void OnDisable()
    {
        FuseEvents.GetFinished -= GetFinished;
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        foreach (var fuse in fuses)
        {
            fuse.SetActive(false);
        }
    }

    protected override void EndPuzzle()
    {
        base.EndPuzzle();
        _collider.enabled = false;
    }

    public override void InteractPuzzle()
    {
        fuses[_currentCount].SetActive(true);
        _currentCount++;
        SoundManager.PlaySound3D(fuseInputSound, transform);
        InteractionSystem.InteractionEvents.DisableInteractionIcon();
            
        if (_currentCount == count)
        {
            EndPuzzle();
        }
    }

    private bool GetFinished()
    {
        return isFinished;
    }
    
    public static class FuseEvents
    {
        public static Func<bool> GetFinished;
    }
}