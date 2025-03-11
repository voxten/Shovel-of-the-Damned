using System;
using System.Collections;
using UnityEngine;

public class PipePuzzle : PuzzleObject
{
    [SerializeField] private GameObject pipesHolder;
    [SerializeField] private GameObject pipeCollider;
    [SerializeField] private Sound pipeRotateSound;
    [SerializeField] private GameObject fuse;
 
    private Pipe _pipeScript;
    private Collider _pipeCollider;

    private bool _isGood;

    private void Awake()
    {
        _pipeCollider = pipeCollider.GetComponent<Collider>();
    }
    
    private void OnEnable()
    {
        PipePuzzleActions.CheckRotations += CheckRotations;
        PipePuzzleActions.GetPipeSound += GetPipeSound;
    }

    private void OnDisable()
    {
        PipePuzzleActions.CheckRotations -= CheckRotations;
        PipePuzzleActions.GetPipeSound -= GetPipeSound;
    }
    
    public override void OpenPuzzle()
    {
        _pipeCollider.enabled = false;
        Narration.DisplayText?.Invoke("The pipes are improperly positioned...");
    }

    protected override void EndPuzzle()
    {
        base.EndPuzzle();
        _pipeCollider.enabled = true;
        Narration.DisplayText?.Invoke("Huh... I heard something in the pipes, I wonder what is it...");
        fuse.SetActive(true);
    }

    public override void QuitPuzzle()
    {
        
    }
    
    private void CheckRotations()
    {
        _isGood = true;
        foreach (Transform child in pipesHolder.transform)
        {
            _pipeScript = child.GetComponent<Pipe>();
            
            var currentZ = NormalizeAngle(child.transform.rotation.eulerAngles.z);
            var correctZ = NormalizeAngle(_pipeScript.correctRotation);
            
            if (Mathf.Round(currentZ) != Mathf.Round(correctZ))
            {
                Debug.Log(child.name + " sie zesrało");
                _isGood = false;
            }
        }

        if (_isGood)
        {
            EndPuzzle();
        }
        else
        {
        }
    }

    private Sound GetPipeSound()
    {
        return pipeRotateSound;
    }
    
    private float NormalizeAngle(float angle)
    {
        return (angle >= 0) ? angle : angle + 360;
    }
    
    public static class PipePuzzleActions
    {
        public static Action CheckRotations;
        public static Func<Sound> GetPipeSound;
    }
}
