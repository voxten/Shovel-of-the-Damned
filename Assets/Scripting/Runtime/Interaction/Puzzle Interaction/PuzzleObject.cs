using System.Collections;
using UnityEngine;

public abstract class PuzzleObject : MonoBehaviour
{
    [Header("Finishing Puzzle Parameters")]
    [SerializeField] private float finishingTime;
    [SerializeField] private Sound finishAudioClip;
    [SerializeField] private Vector2 clipVolume = new Vector2(1,1);
    public bool isFinished;
    
    // function which indicates what should be done after entering a puzzle
    public abstract void OpenPuzzle();
    
    // function which indicates what should be done after finishing a puzzle
    protected virtual void EndPuzzle()
    {
        isFinished = true;
        Debug.Log("Puzzle has been solved");
        StartCoroutine(WaitForEndOfPuzzle(finishingTime));
    }
    
    // function which indicates what should be done after we quit puzzle without finishing
    public abstract void QuitPuzzle();
    
    private IEnumerator WaitForEndOfPuzzle(float time)
    {
        yield return new WaitForSeconds(time);
        SoundManager.PlaySound3D(finishAudioClip,transform, new Vector2(1,1), clipVolume);
    }
}
