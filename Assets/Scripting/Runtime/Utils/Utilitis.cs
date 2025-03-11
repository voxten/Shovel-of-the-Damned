using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilitis
{
    public static void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
