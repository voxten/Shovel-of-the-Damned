using UnityEngine;

public class ItemCamera : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    
    
    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ChangeCameraEnable();
        }
        
    }

    private void ChangeCameraEnable()
    {
        camera.SetActive(!camera.activeSelf);
    }

    
}
