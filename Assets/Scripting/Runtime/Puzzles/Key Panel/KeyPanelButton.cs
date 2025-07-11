using UnityEngine;
using DG.Tweening;

public class KeyPanelButton : MonoBehaviour
{
    [SerializeField] private int number;
    private const float StandardButton = -0.30f;  
    private const float PressedButton = -0.25f;

    private void OnMouseOver()
    {
        Debug.Log("Even goes in??");
        if (Input.GetMouseButtonDown(0))
        {
            var position = transform.localPosition;
            
            gameObject.transform.DOLocalMove(new Vector3(PressedButton, position.y, position.z), 0.25f)
                .SetEase(Ease.OutExpo)
                .OnComplete(() =>
                {
                    gameObject.transform.DOLocalMove(new Vector3(StandardButton, position.y, position.z), 0.25f)
                        .SetEase(Ease.OutExpo);
                });
            
            KeyPanelPuzzle.KeyPanelEvents.AddNumber(number);
        }
    }
}