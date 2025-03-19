using TMPro;
using UnityEngine;

public class ItemCameraOptions : MonoBehaviour
{
    [SerializeField] private GameObject nightVision;
    [SerializeField] private TextMeshProUGUI showedBateryLevel;
    private double batterryLevel = 100.00;
    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ChangeNightVisionEnable();
        }
        lowerBateryState();
    }

    private void ChangeNightVisionEnable()
    {
        nightVision.SetActive(!nightVision.activeSelf);
    }

    private void lowerBateryState()
    {
        if (nightVision.activeSelf)
        {
            batterryLevel -= 0.01;
        }
        else
        {
            batterryLevel -= 0.001;
        }
        int toShow = System.Convert.ToInt32(System.Math.Floor(batterryLevel));
        showedBateryLevel.text = toShow + "%";
    }
}
