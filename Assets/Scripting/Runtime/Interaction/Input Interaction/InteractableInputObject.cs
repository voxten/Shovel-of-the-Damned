using System.Collections;
using UnityEngine;

public abstract class InteractableInputObject : MonoBehaviour
{
    [Header("Finishing Puzzle Parameters")]
    [SerializeField] private float finishingTime;
    [SerializeField] private Sound finishAudioClip;
    public bool isFinished;
    
    // function which indicates what should be done after finishing a puzzle
    protected virtual void EndPuzzle()
    {
        isFinished = true;
        Debug.Log("Puzzle has been solved");
        StartCoroutine(WaitForEndOfPuzzle(finishingTime));
    }

    public abstract void InteractPuzzle();

    private IEnumerator WaitForEndOfPuzzle(float time)
    {
        yield return new WaitForSeconds(time);
        SoundManager.PlaySound3D(finishAudioClip,transform);
    }
}