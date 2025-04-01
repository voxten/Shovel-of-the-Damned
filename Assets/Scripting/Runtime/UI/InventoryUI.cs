using System;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Viewer")]
    [SerializeField] private RectTransform inventoryViewer;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private float highlightValue;
    
    [Header("ButtonPanel")]
    [SerializeField] private RectTransform buttonParent;
    [SerializeField] private GameObject buttonPrefab;
    
    private List<Item> _itemList;
    private int _selectedIndex;
    private List<GameObject> _displayedItems = new();
    private const int VisibleItemsCount = 3;

    private void OnEnable()
    {
        Utilitis.SetCursorState(false);
        GenerateButtons();
        FirstPersonController.PlayerEvents.ToggleMove();
        Time.timeScale = 0;
        
        itemNameText.enabled = false;
        itemDescText.enabled = false;
        inventoryViewer.gameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        Utilitis.SetCursorState(true);
        DeleteButtons();
        FirstPersonController.PlayerEvents.ToggleMove();
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && _displayedItems.Count != 0)
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D) && _displayedItems.Count != 0)
        {
            MoveRight();
        }
        else if (Input.GetKeyDown(KeyCode.F) && _itemList.Count > 0)
        {
            UseSelectedItem();
        }
    }
    
    private void MoveLeft()
    {
        if (NoteUIManager.NoteActions.GetIsOn()) return;
        _selectedIndex = (_selectedIndex - 1 + _itemList.Count) % _itemList.Count;
        AnimateInventoryDisplay();
    }

    private void MoveRight()
    {
        if (NoteUIManager.NoteActions.GetIsOn()) return;
        _selectedIndex = (_selectedIndex + 1) % _itemList.Count;
        AnimateInventoryDisplay();
    }
    
    private void UseSelectedItem()
    {
        if (_selectedIndex < 0 || _selectedIndex >= _itemList.Count)
            return;

        Item item = _itemList[_selectedIndex];
        if (item is NoteItem note && !NoteUIManager.NoteActions.GetIsOn())
        {
            NoteUIManager.NoteActions.OpenNote(note);
        }
    }
    
    private void SetInventoryPanel(ItemType itemType)
    {
        if (inventoryViewer.childCount != 0)
        {
            foreach (Transform child in inventoryViewer)
            {
                Destroy(child.gameObject);
            }
        }

        var allItems = Inventory.InventoryEvents.GetItemsByType(itemType);
        if (allItems.Count == 0) return;

        // Dictionary to track occurrences
        Dictionary<Item, int> itemOccurrences = new();
        foreach (var item in allItems)
        {
            if (itemOccurrences.ContainsKey(item))
                itemOccurrences[item]++;
            else
                itemOccurrences[item] = 1;
        }

        _itemList = new List<Item>(itemOccurrences.Keys); // Store unique items

        if (_itemList.Count != 0)
        {
            itemNameText.enabled = true;
            itemDescText.enabled = true;
            inventoryViewer.gameObject.SetActive(true);

            _displayedItems.Clear();
            var displayCount = Mathf.Min(_itemList.Count, VisibleItemsCount);
            for (var i = 0; i < displayCount; i++)
            {
                var itemTemp = Instantiate(inventoryItemPrefab, inventoryViewer);
                _displayedItems.Add(itemTemp);
            }
        }

        UpdateInventoryDisplay(itemOccurrences);
    }


    private void UpdateInventoryDisplay(Dictionary<Item, int> itemOccurrences)
    {
        var displayCount = Mathf.Min(_itemList.Count, VisibleItemsCount);

        for (var i = 0; i < displayCount; i++)
        {
            var itemIndex = (_selectedIndex + i - 1 + _itemList.Count) % _itemList.Count;
            var item = _itemList[itemIndex];
            var itemTransform = _displayedItems[i].transform;

            // Always use itemIndex for the sprite to show the correct item
            itemTransform.GetComponent<Image>().sprite = item.itemIcon;

            // Get the TextMeshPro in the prefab and set the count text
            var countText = itemTransform.GetComponentInChildren<TextMeshProUGUI>();
            if (itemOccurrences.TryGetValue(item, out int count))
            {
                countText.text = count > 1 ? count.ToString() : "";
            }
            else
            {
                countText.text = "";
            }

            switch (displayCount)
            {
                case 1:
                    itemTransform.localScale = Vector3.one * 1.2f;
                    itemTransform.localPosition = Vector3.zero;
                    break;
                case 2:
                    itemTransform.localScale = (itemIndex == _selectedIndex) ? Vector3.one * 1.2f : Vector3.one;
                    itemTransform.localPosition = new Vector3((i - 0.5f) * 200, 0, 0);
                    break;
                default:
                    itemTransform.localScale = (i == 1) ? Vector3.one * 1.2f : Vector3.one;
                    itemTransform.localPosition = new Vector3((i - 1) * 200, 0, 0);
                    break;
            }
        }
        SetItemDesc(_selectedIndex); // Always show description for selected item
    }


    private void SetItemDesc(int itemIndex)
    {
        itemNameText.text = _itemList[itemIndex].itemName;
        itemDescText.text = _itemList[itemIndex].itemDescription;
    }

    private void AnimateInventoryDisplay()
    {
        var displayCount = Mathf.Min(_itemList.Count, VisibleItemsCount);
        for (var i = 0; i < displayCount; i++)
        {
            var itemTransform = _displayedItems[i].transform;
            
            if (_itemList.Count == 2)
            {
                itemTransform.localScale = (i == _selectedIndex) ? Vector3.one * 1.2f : Vector3.one;
            }
            else
            {
                var itemIndex = (_selectedIndex + i - 1 + _itemList.Count) % _itemList.Count;
                itemTransform.DOLocalMoveX((i - 1) * 200, 0.3f).SetEase(Ease.OutQuad);
                itemTransform.DOScale((i == 1) ? Vector3.one * 1.2f : Vector3.one, 0.3f);
                itemTransform.GetComponent<Image>().sprite = _itemList[itemIndex].itemIcon;
            }
        }
        SetItemDesc(_selectedIndex);
        SetInventoryPanel(_itemList[_selectedIndex].itemType);
    }

    private void GenerateButtons()
    {
        var tempList = Inventory.InventoryEvents.GetAllItems();
        if (tempList == null || tempList.Count == 0)
            return;
        var uniqueItemTypes = new HashSet<ItemType>();
        foreach (var item in tempList)
        {
            if (!uniqueItemTypes.Add(item.itemType)) 
                continue;

            var buttonObject = Instantiate(buttonPrefab, buttonParent);
            var button = buttonObject.GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = item.itemType.ToString().ToUpper();
            button.onClick.AddListener(() => SetInventoryPanel(item.itemType));
        }
    }
    
    private void DeleteButtons()
    {
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }
    }
}
