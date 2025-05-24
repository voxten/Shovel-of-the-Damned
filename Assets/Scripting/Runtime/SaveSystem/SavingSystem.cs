using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using static StarterAssets.FirstPersonController;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;

public class SavingSystem : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;
    private List<String> _allPicable = new List<string>();

    private void Start()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int interactionLayer = LayerMask.NameToLayer("Interaction");
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == interactionLayer)
            {
                PickItem pickItem = obj.GetComponent<PickItem>();
                if (pickItem != null && !string.IsNullOrEmpty(pickItem.ID))
                {
                        _allPicable.Add(pickItem.ID);
                }
            }
        }
        //string idCard = "76941d22-6c05-422a-922d-ab802319afe0";
        //Debug.Log("Czy jest w pierwotnej: " + _allPicable.IndexOf(idCard));
    }

    public static class SavingSystemEvents
    {
        public static Action Save;
        public static Action Load;
    }
    private void OnEnable()
    {
        SavingSystemEvents.Save += Save;
        SavingSystemEvents.Load += Load;
    }

    private void OnDisable()
    {
        SavingSystemEvents.Save -= Save;
        SavingSystemEvents.Load -= Load;
    }

    private void Save()
    {
        SaveData saveData = CreateSaveDataGameObject();

        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "Save.txt");
        bf.Serialize(fileStream, saveData);
        fileStream.Close();
    }

    private SaveData CreateSaveDataGameObject()
    {
        SaveData saveData = new SaveData();

        saveData.playerPositionX = playerTransform.position.x;
        saveData.playerPositionY = playerTransform.position.y;
        saveData.playerPositionZ = playerTransform.position.z;

        saveData.playerRotationX = playerTransform.rotation.x;
        saveData.playerRotationY = playerTransform.rotation.y;
        saveData.playerRotationZ = playerTransform.rotation.z;
        saveData.playerRotationW = playerTransform.rotation.w;


        saveData.cameraPositionX = cameraTransform.position.x;
        saveData.cameraPositionY = cameraTransform.position.y;
        saveData.cameraPositionZ = cameraTransform.position.z;

        saveData.cameraRotationX = cameraTransform.rotation.x;
        saveData.cameraRotationY = cameraTransform.rotation.y;
        saveData.cameraRotationZ = cameraTransform.rotation.z;
        saveData.cameraRotationW = cameraTransform.rotation.w;

        saveData.pickabelsIDs = FindAllPickable();
        saveData.inventory = new Dictionary<string, int>();

        foreach (Item item in Inventory.InventoryEvents.GetAllItems())
        {
            if (saveData.inventory.ContainsKey(item.Id))
            {
                saveData.inventory[item.Id] += 1;
            }
            else
            {
                saveData.inventory[item.Id] = 1;
            }
            if(item.name == "Key")
            {
                saveData.arm = true;
            }
            
        }

        saveData.batteryLevel = FlashlightOptions.FlashlightOptionsEvents.GetBatteryLevel();

        saveData.radio = FindObjectOfType<RadioPuzzle>().isFinished;
        saveData.generator = FindObjectOfType<GeneratorPuzzle>().isFinished;

        if (saveData.arm)
        {
            GameObject arm = GameObject.Find("Arm");
            Transform armTransform = arm.transform;

            saveData.armPositionX = armTransform.position.x;
            saveData.armPositionY = armTransform.position.y;
            saveData.armPositionZ = armTransform.position.z;

            saveData.armRotationX = armTransform.rotation.x;
            saveData.armRotationY = armTransform.rotation.y;
            saveData.armRotationZ = armTransform.rotation.z;
            saveData.armRotationW = armTransform.rotation.w;
        }

        saveData.morgue = FindObjectOfType<MorgueDoorCorrect>().isFinished;

        //Debug.Log(saveData.pickabelsIDs[0]);
        return saveData;
    }

    private void Load()
    {
        if(File.Exists(Application.persistentDataPath + "Save.txt"))
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("MainScene");

        }
        else
        {
            Debug.Log("There is no save file!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        GameObject cameraObj = GameObject.FindWithTag("MainCamera");
        Transform playerTransform = playerObj.transform;
        Transform cameraTransform = cameraObj.transform;
        NoteItem note = new NoteItem();
        NoteUIManager.NoteActions.OpenNote(note);
        SceneManager.sceneLoaded -= OnSceneLoaded;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(Application.persistentDataPath + "Save.txt", FileMode.Open);

        SaveData saveData = bf.Deserialize(fileStream) as SaveData;
        fileStream.Close();

        playerTransform.position = new Vector3(saveData.playerPositionX, saveData.playerPositionY, saveData.playerPositionZ);
        playerTransform.rotation = new Quaternion(saveData.playerRotationX, saveData.playerRotationY, saveData.playerRotationZ, saveData.playerRotationW);

        cameraTransform.position = new Vector3(saveData.cameraPositionX, saveData.cameraPositionY, saveData.cameraPositionZ);
        cameraTransform.rotation = new Quaternion(saveData.cameraRotationX, saveData.cameraRotationY, saveData.cameraRotationZ, saveData.cameraRotationW);

        DiasablePckable(saveData.pickabelsIDs);
        DiasablePckable(saveData.pickabelsIDs);

        foreach (var item in saveData.inventory)
        {
            Item item1 = FindItemById(item.Key);
            if (item1 != null)
            {
                if (item1.itemType == ItemType.Notes)
                {
                    NoteItem noteItem = FindNoteItemById(item1.Id);
                    Inventory.InventoryEvents.AddItem(noteItem);
                }
                else
                {
                    for (int i = 0; i < item.Value; i++)
                    {
                        Inventory.InventoryEvents.AddItem(item1);
                    }
                }
            }
            
        }

        FlashlightOptions.FlashlightOptionsEvents.SetBatteryLevel(saveData.batteryLevel);

        if (saveData.radio)
        {
            FindObjectOfType<RadioPuzzle>().isFinished = true;
            FindObjectOfType<RadioPuzzle>().isAfterLoad = true;
        }
        if (saveData.generator)
        {
            FindObjectOfType<GeneratorPuzzle>().isFinished = true;
            FindObjectOfType<GeneratorPuzzle>().isAfterLoad = true;
        }
        if (saveData.arm)
        {
            GameObject arm = GameObject.Find("Arm");
            Transform armTransform = arm.transform;
            armTransform.position = new Vector3(saveData.armPositionX, saveData.armPositionY, saveData.armPositionZ);
            armTransform.rotation = new Quaternion(saveData.armRotationX, saveData.armRotationY, saveData.armRotationZ, saveData.armRotationW);

            arm.GetComponentInChildren<IceMelting>().gameObject.SetActive(false);
            //arm.GetComponentInChildren<PickItem>().gameObject.SetActive(false);
        }
        if (saveData.morgue)
        {
            FindObjectOfType<MorgueDoorCorrect>().isFinished = true;
        }
    }

    private List<string> FindAllPickable()
    {
        List<string> pickable = _allPicable;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int interactionLayer = LayerMask.NameToLayer("Interaction");
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == interactionLayer)
            {
                PickItem pickItem = obj.GetComponent<PickItem>();
                if (pickItem != null && !string.IsNullOrEmpty(pickItem.ID))
                {
                    pickable.Remove(pickItem.ID);
                }
            }
        }

        return pickable;
    }

    private void DiasablePckable(List<string> pickableIDs)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int interactionLayer = LayerMask.NameToLayer("Interaction");
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == interactionLayer)
            {
                PickItem pickItem = obj.GetComponent<PickItem>();
                if (pickItem != null && !string.IsNullOrEmpty(pickItem.ID))
                {
                    int index = pickableIDs.IndexOf(pickItem.ID);

                    if(index != -1)
                    {
                        obj.SetActive(false);
                        pickableIDs.RemoveAt(index);

                    }
                }
            }
        }
    }

    private Item FindItemById(string id)
    {
        Item[] allItems = Resources.LoadAll<Item>("Items");
        foreach (var item in allItems)
        {
            if (item.Id == id)
            {
                return item;
            }
        }
        Debug.LogWarning($"Item with ID {id} not found!");
        return null;
    }

    private NoteItem FindNoteItemById(string id)
    {
        NoteItem[] allItems = Resources.LoadAll<NoteItem>("Items/Files");
        foreach (var item in allItems)
        {
            if (item.Id == id)
            {
                return item;
            }
        }
        Debug.LogWarning($"Item with ID {id} not found!");
        return null;
    }
}
