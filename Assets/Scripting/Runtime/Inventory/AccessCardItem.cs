using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AccessCardItem", menuName = "ScriptableObjects/AccessCardItem", order = 0)]
public class AccessCardItem : Item
{
    public AccessCardPair cardPair;

    public void SetNewAccessCard(AccessCardPair accessCardPair)
    {
        cardPair = accessCardPair;
        itemName = cardPair.description;
    }
}

