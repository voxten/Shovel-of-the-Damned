using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
        }
    }
}
