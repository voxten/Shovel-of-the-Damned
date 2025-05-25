using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //Player Model
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    public float playerRotationX;
    public float playerRotationY;
    public float playerRotationZ;
    public float playerRotationW;
    //Player Camera

    public float cameraPositionX;
    public float cameraPositionY;
    public float cameraPositionZ;

    public float cameraRotationX;
    public float cameraRotationY;
    public float cameraRotationZ;
    public float cameraRotationW;

    //Pickable items
    public int pickableValue;
    public List<string> pickabelsIDs;

    //Inventory
    public Dictionary<string,int> inventory;

    //Battery level
    public float batteryLevel;

    //Puzzles
    public bool radio;
    public bool generator;
    public bool arm;
    public bool morgue;

    //Arm Puzle
    public float armPositionX;
    public float armPositionY;
    public float armPositionZ;

    public float armRotationX;
    public float armRotationY;
    public float armRotationZ;
    public float armRotationW;

    //Card
    public int cardLevel;
}
