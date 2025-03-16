using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemTypeButton : MonoBehaviour
{
    protected Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }
}
