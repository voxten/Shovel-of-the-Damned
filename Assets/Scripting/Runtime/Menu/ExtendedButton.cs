using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ExtendedButton : MonoBehaviour
{
    protected Button _extendedButton;
    
    protected virtual void Awake()
    {
        _extendedButton = GetComponent<Button>();
    }
    
    protected virtual void OnEnable()
    {
        _extendedButton.onClick.AddListener(Submit);
    }

    protected virtual void OnDisable()
    {
        _extendedButton.onClick.RemoveListener(Submit);
    }
    
    protected virtual void Submit()
    {
        //
    }
}
