using TMPro;
using UnityEngine;

public class ItemCameraOptions : MonoBehaviour
{
    [SerializeField] private GameObject nightVision;
    [SerializeField] private TextMeshProUGUI showedBateryLevel;
    [SerializeField] private Material screen;
    private float _batterryLevel = 100.00f;
    private void Start()
    {
        screen.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ChangeNightVisionEnable();
        }
        LowerBateryState(); //Tymczasowe oparte na frame'ach
    }

    private void ChangeNightVisionEnable()
    {
        nightVision.SetActive(!nightVision.activeSelf);
    }

    private void LowerBateryState()
    {
        if (nightVision.activeSelf)
        {
            _batterryLevel -= 0.01f;
        }
        else
        {
            _batterryLevel -= 0.001f;
        }
        int toShow = System.Convert.ToInt32(System.Math.Floor(_batterryLevel));
        showedBateryLevel.text = toShow + "%";
        if(_batterryLevel < 0.0)
        {
            screen.color = Color.black;
        }
    }
}
