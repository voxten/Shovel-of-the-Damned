using UnityEngine;

public class ItemCamera : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] private Item itemCamera;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && Inventory.InventoryEvents.FindItem(itemCamera))
        {
            ChangeCameraEnable();
        }
        
    }

    private void ChangeCameraEnable()
    {
        camera.SetActive(!camera.activeSelf);
    }

    
}
