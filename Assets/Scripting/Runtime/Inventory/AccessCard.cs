using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AccessCardData", menuName = "ScriptableObjects/AccessCardData", order = 0)]
public class AccessCardData : ScriptableObject
{
    public List<AccessCardPair> accessCards;
}

[Serializable]
public struct AccessCardPair
{
    public AccessCardType cardType;
    public string description;
    public String upgradeCode;
}

public enum AccessCardType
{
    Level0,
    Level1,
    Level2,
    Level3,
    Level4
}