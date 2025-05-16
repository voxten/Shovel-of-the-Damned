using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PuzzleTutorial", menuName = "ScriptableObjects/PuzzleTutorial", order = 0)]
public class PuzzleTutorialData : ScriptableObject
{
    public List<PuzzleTutorialPair> puzzleTutorials;
}

[Serializable]
public struct PuzzleTutorialPair
{
    public PuzzleIcon puzzleIcon;
    public string description;
}

public enum PuzzleIcon
{
    Mouse0,
    Mouse1,
    Scroll,
}