using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KeyPanelButtonEnter : MonoBehaviour
{
    private const float StandardButton = -0.24f;  
    private const float PressedButton = -0.25f;
    
    private void OnMouseOver()
    {
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

            KeyPanelPuzzle.KeyPanelEvents.CheckPasscode();
        }
    }
}
