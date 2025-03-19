using UnityEngine;
using DG.Tweening;

public class KeyPanelButtonClear : MonoBehaviour
{
    [SerializeField] private Sound clearSound;
    private const float StandardButton = -0.30f;  
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

            KeyPanelPuzzle.KeyPanelEvents.ClearPasscode();
            SoundManager.PlaySound3D(clearSound, transform, Vector2.one, new Vector2(0.6f,0.7f));
        }
    }
}