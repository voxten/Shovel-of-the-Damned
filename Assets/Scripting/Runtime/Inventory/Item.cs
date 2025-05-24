using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 0)]
public class Item : ScriptableObject
{

    public Sprite itemIcon;
    public string itemName;
    public string itemDescription;
    public ItemType itemType;


    private string GenerateUniqueID()
    {
        return Guid.NewGuid().ToString("N"); // Generates a 32-character unique string
    }

    public string Id = Guid.NewGuid().ToString(); // generowane automatycznie

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.NewGuid().ToString();
        }
    }
#endif
}

public enum ItemType
{
    keyItem,
    Personal,
    Notes,
}