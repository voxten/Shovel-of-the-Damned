using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SavingSystem : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;
    private List<String> _allPicable = new List<string>();
    
    [Header("Animation Settings")]
    [SerializeField] private Image loadingCircle;
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float fillAnimationDuration = 0.8f;
    [SerializeField] private AccessCardData accessCardData;

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
        
        // Initialize the loading circle as invisible
        if (loadingCircle != null)
        {
            loadingCircle.color = new Color(loadingCircle.color.r, loadingCircle.color.g, loadingCircle.color.b, 0);
            loadingCircle.gameObject.SetActive(false);
        }
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
        StartCoroutine(SaveWithAnimation());
    }
    
    private IEnumerator SaveWithAnimation()
    {
        // Show and fade in the loading circle
        if (loadingCircle != null)
        {
            // Reset values
            loadingCircle.fillAmount = 0;
            loadingCircle.gameObject.SetActive(true);
            loadingCircle.DOFade(1f, fadeDuration);
            
            // Start infinite rotation
            loadingCircle.transform.DORotate(new Vector3(0, 0, -360), rotationDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            
            // Animate fillAmount from 0 to 1 in a loop
            loadingCircle.DOFillAmount(1, fillAnimationDuration)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Restart);
        }

        // Create save data
        SaveData saveData = CreateSaveDataGameObject();

        // Save the data
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "Save.txt");
        bf.Serialize(fileStream, saveData);
        fileStream.Close();

        // Wait a minimum time so the animation is visible
        yield return new WaitForSeconds(0.5f);

        // Fade out and hide the loading circle
        if (loadingCircle != null)
        {
            loadingCircle.DOFade(0f, fadeDuration).OnComplete(() => 
            {
                loadingCircle.gameObject.SetActive(false);
                loadingCircle.transform.DOKill();
                loadingCircle.DOKill(); // Stop the fill animation
                loadingCircle.fillAmount = 0; // Reset for next time
            });
        }
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
            if (item == null || string.IsNullOrEmpty(item.Id))
            {
                Debug.LogWarning($"Skipping invalid item in inventory: {(item == null ? "NULL" : item.name)}");
                continue;
            }
            
            if (!saveData.inventory.TryAdd(item.Id, 1))
            {
                saveData.inventory[item.Id] += 1;
            }

            if(item.name == "Key")
            {
                saveData.arm = true;
            }
        }

        saveData.batteryLevel = FindFirstObjectByType<FlashlightOptions>(FindObjectsInactive.Include).batteryLevel;

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

        AccessCardItem accessCard = (AccessCardItem)Inventory.InventoryEvents.GetAccessCard();
        switch (accessCard.cardPair.cardType)
        {
            case AccessCardType.Level0:
                saveData.cardLevel = 0;
                break;
            case AccessCardType.Level1: 
                saveData.cardLevel = 1;
                break;
            case AccessCardType.Level2:
                saveData.cardLevel = 2;
                break;
            case AccessCardType.Level3:
                saveData.cardLevel = 3;
                break;
            case AccessCardType.Level4:
                saveData.cardLevel = 4;
                break;
            default: break;
        }

        saveData.flashlightTutorial = FindFirstObjectByType<FlashlightOptions>(FindObjectsInactive.Include).tutorialCompleted;
        //Debug.Log(saveData.pickabelsIDs[0]);
        return saveData;
    }

    private void Load()
    {
        if(File.Exists(Application.persistentDataPath + "Save.txt"))
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneLoader.SceneEvents.AnimateLoadScene("MainScene");
            Utilitis.SetCursorState(true);
        }
        else
        {
            Debug.Log("There is no save file!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshPickableList();
        PickAccessCard pickAccessCard = FindFirstObjectByType<PickAccessCard>(FindObjectsInactive.Exclude);
        if(pickAccessCard != null)
        {
            pickAccessCard.gameObject.SetActive(false);
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        GameObject cameraObj = GameObject.FindWithTag("MainCamera");
        Transform playerTransform = playerObj.transform;
        Transform cameraTransform = cameraObj.transform;
        //NoteItem note = new NoteItem();
        //NoteUIManager.NoteActions.OpenNote(note);
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
        Inventory.InventoryEvents.RemoveStartupItems();

        FindFirstObjectByType<FlashlightOptions>(FindObjectsInactive.Include).batteryLevel = saveData.batteryLevel;

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
        FindFirstObjectByType<EnemyAI>(FindObjectsInactive.Include).gameObject.SetActive(true);

        AccessCardItem accessCard = (AccessCardItem)Inventory.InventoryEvents.GetAccessCard();
        accessCard.SetNewAccessCard(accessCardData.accessCards[saveData.cardLevel]);

        FlashlightOptions flashlightOptions = FindFirstObjectByType<FlashlightOptions>(FindObjectsInactive.Include);
        flashlightOptions.tutorialCompleted = saveData.flashlightTutorial;
        flashlightOptions.isOnce = saveData.flashlightTutorial;

    }

    private List<string> FindAllPickable()
    {
        List<string> pickable = new List<string>();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int interactionLayer = LayerMask.NameToLayer("Interaction");
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == interactionLayer)
            {
                PickItem pickItem = obj.GetComponent<PickItem>();
                if (pickItem != null && !string.IsNullOrEmpty(pickItem.ID))
                {
                    pickable.Add(pickItem.ID); // Dodaj ID aktywnych przedmiotów
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
                    if (!pickableIDs.Contains(pickItem.ID)) // Wy³¹cz przedmioty, których ID nie ma na liœcie
                    {
                        obj.SetActive(false);
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

    private void RefreshPickableList()
    {
        _allPicable.Clear();
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
    }
}
