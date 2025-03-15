using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 0)]
public class Item : ScriptableObject
{
    public string id;
    public Sprite itemIcon;
    public string itemName;
    public string itemDescription;
    public ItemType itemType;
}


public enum ItemType
{
    test,
    test2,
    test3,
}
