using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 0)]
public class Item : ScriptableObject
{
    [SerializeField] private string id;

    public string Id { get; private set; }

    public Sprite itemIcon;
    public string itemName;
    public string itemDescription;
    public ItemType itemType;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = GenerateUniqueID();
        }
        
        Id = id;
    }

    private string GenerateUniqueID()
    {
        return Guid.NewGuid().ToString("N"); // Generates a 32-character unique string
    }
}

public enum ItemType
{
    test,
    test2,
    test3,
    keyItem,
    Personal
}