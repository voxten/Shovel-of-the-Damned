using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoteItem", menuName = "ScriptableObjects/NoteItem", order = 0)]
public class NoteItem : Item
{
    [TextArea(3, 10)]
    public List<string> noteContent;
}
