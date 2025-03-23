using System;
using UnityEngine;

public class RadioPuzzle : PuzzleObject
{
    [SerializeField] private RadioScroll[] radioScrolls;

    private void OnEnable()
    {
        RadioEvents.CheckNeedles += CheckNeedles;
        RadioEvents.GetIsFinished += GetIsFinished;
    }

    private void OnDisable()
    {
        RadioEvents.CheckNeedles -= CheckNeedles;
        RadioEvents.GetIsFinished -= GetIsFinished;
    }

    public override void OpenPuzzle()
    {

    }

    public override void QuitPuzzle()
    {

    }

    private bool GetIsFinished()
    {
        return isFinished;
    }

    private void CheckNeedles()
    {
        var count = 0;
        foreach (var scroll in radioScrolls)
        {
            if (scroll.CheckNeedlePosition())
            {
                count++;
            }
        }

        if (count == 2)
        {
            EndPuzzle();
        }
    }

    public static class RadioEvents
    {
        public static Action CheckNeedles;
        public static Func<bool> GetIsFinished;
    }
}
