using UnityEngine;

public class ItemCamera : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject nightVision;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ChangeCameraEnable();
        }
        if (Input.GetKeyDown(KeyCode.Y) && camera.activeSelf)
        {
            ChangeNightVisionEnable();
        }
    }

    private void ChangeCameraEnable()
    {
        camera.SetActive(!camera.activeSelf);
    }

    private void ChangeNightVisionEnable()
    {
        nightVision.SetActive(!nightVision.activeSelf);
    }
}
