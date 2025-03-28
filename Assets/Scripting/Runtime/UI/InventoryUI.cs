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
    }

    private void MoveLeft()
    {
        _selectedIndex = (_selectedIndex - 1 + _itemList.Count) % _itemList.Count;
        AnimateInventoryDisplay();
    }

    private void MoveRight()
    {
        _selectedIndex = (_selectedIndex + 1) % _itemList.Count;
        AnimateInventoryDisplay();
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
        
        _itemList = Inventory.InventoryEvents.GetItemsByType(itemType);
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
        UpdateInventoryDisplay();
    }

    private void UpdateInventoryDisplay()
    {
        var displayCount = Mathf.Min(_itemList.Count, VisibleItemsCount);
        
        for (var i = 0; i < displayCount; i++)
        {
            var itemIndex = (_selectedIndex + i - 1 + _itemList.Count) % _itemList.Count;
            var itemTransform = _displayedItems[i].transform;
            
            itemTransform.GetComponent<Image>().sprite = _itemList.Count == 2 ? _itemList[i].itemIcon : _itemList[itemIndex].itemIcon;
            
            switch (displayCount)
            {
                case 1:
                    itemTransform.localScale = Vector3.one * 1.2f; // Highlight single item
                    itemTransform.localPosition = Vector3.zero; // Center
                    break;
                case 2:
                    itemTransform.localScale = (i == _selectedIndex) ? Vector3.one * 1.2f : Vector3.one;
                    itemTransform.localPosition = new Vector3((i - 0.5f) * 200, 0, 0); // Spread items evenly
                    break;
                default:
                    itemTransform.localScale = (i == 1) ? Vector3.one * 1.2f : Vector3.one; // Highlight middle item
                    itemTransform.localPosition = new Vector3((i - 1) * 200, 0, 0);
                    break;
            }
            SetItemDesc(itemIndex);
        }
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
