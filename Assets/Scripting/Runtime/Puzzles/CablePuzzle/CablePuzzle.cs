using UnityEngine;
using UnityEngine.Events;

public class CablePuzzle : PuzzleObject
{
    [SerializeField] private UnityEvent openPuzzle;
    [SerializeField] private UnityEvent closePuzzle;

    public override void OpenPuzzle()
    {
        openPuzzle.Invoke();
    }

    public override void QuitPuzzle()
    {
        closePuzzle.Invoke();
    }
}
