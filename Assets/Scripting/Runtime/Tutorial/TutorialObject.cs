using UnityEngine;

[CreateAssetMenu(fileName = "TutorialObject", menuName = "ScriptableObjects/TutorialObject", order = 0)]
public class TutorialObject : ScriptableObject 
{
    public KeyCode keyCode;
    public string tutorialDescription;
}