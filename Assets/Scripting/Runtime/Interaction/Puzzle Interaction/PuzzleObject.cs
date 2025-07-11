using System.Collections;
using UnityEngine;

public abstract class PuzzleObject : MonoBehaviour
{
    [Header("Finishing Puzzle Parameters")]
    [SerializeField] private float finishingTime;
    [SerializeField] protected Sound finishAudioClip;
    [SerializeField] protected float finishAudioVolume = 1f;
    [SerializeField] private bool customFinish;
    [SerializeField] private GameObject buttonPanel;
    public bool isFinished;
    public bool isAfterLoad;
    
    private Coroutine _currentCoroutine;

    // function which indicates what should be done after entering a puzzle
    public virtual void OpenPuzzle()
    {
        FindFirstObjectByType<FlashlightOptions>().inPuzzle = true;
        buttonPanel.SetActive(true);
    }
    
    // function which indicates what should be done after finishing a puzzle
    protected virtual void EndPuzzle()
    {
        isFinished = true;
        Debug.Log("Puzzle has been solved");
        SavingSystem.SavingSystemEvents.Save();
        StartCoroutine(WaitForEndOfPuzzle(finishingTime));
    }
    
    // function which indicates what should be done after we quit puzzle without finishing
    public virtual void QuitPuzzle()
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);
        _currentCoroutine = StartCoroutine(WaitAfterExit());
    }

    private IEnumerator WaitAfterExit()
    {
        buttonPanel.SetActive(false);
        yield return new WaitForSeconds(1);
        FindFirstObjectByType<FlashlightOptions>().inPuzzle = false;
    }
    
    private IEnumerator WaitForEndOfPuzzle(float time)
    {
        yield return new WaitForSeconds(time);
        if (isFinished && !customFinish)
        {
            InteractionSystem.InteractionEvents.ExitPuzzleInteraction();
            buttonPanel.SetActive(false);
            GetComponent<PuzzleInteraction>().puzzleCollider.enabled = false;
        }
        SoundManager.PlaySound3D(finishAudioClip,transform, null, finishAudioVolume);
    }
}
