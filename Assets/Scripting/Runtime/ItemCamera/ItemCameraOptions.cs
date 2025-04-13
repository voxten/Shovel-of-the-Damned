using TMPro;
using UnityEngine;

public class ItemCameraOptions : MonoBehaviour
{
    [SerializeField] private GameObject nightVision;
    [SerializeField] private TextMeshPro showedBateryLevel;
    [SerializeField] private Material screen;
    [SerializeField] private Item itemBattery;
    [SerializeField] private GameObject light;
    private float _batterryLevel = 2.00f;
    private float _lastLowerTime;
    
    private void Start()
    {
        screen.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        _lastLowerTime = Time.time;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ChangeNightVisionEnable();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeBattery();
        }
        LowerBateryState(); //Tymczasowe oparte na frame'ach
    }

    private void ChangeNightVisionEnable()
    {
        nightVision.SetActive(!nightVision.activeSelf);
        light.SetActive(nightVision.activeSelf);
    }

    private void LowerBateryState()
    {
        if(Time.time - _lastLowerTime > 0.1f)
        {
            _lastLowerTime = Time.time;
            if (nightVision.activeSelf)
            {
                _batterryLevel -= 0.1f;
            }
            else
            {
                _batterryLevel -= 0.05f;
            }
        }
        int toShow = System.Convert.ToInt32(System.Math.Floor(_batterryLevel));
        showedBateryLevel.text = toShow + "%";
        if (_batterryLevel< 0.0)
        {
            screen.color = Color.black;
            showedBateryLevel.text = "";
        }
    }

    private void ChangeBattery()
    {
        if (Inventory.InventoryEvents.FindItem(itemBattery) && _batterryLevel <= 25f)
        {
            Inventory.InventoryEvents.RemoveItem(itemBattery);
            _batterryLevel = 100.00f;
            screen.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
    }
}
