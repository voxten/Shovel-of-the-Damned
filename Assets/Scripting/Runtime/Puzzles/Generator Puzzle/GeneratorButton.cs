using UnityEngine;
using DG.Tweening;

public class GeneratorButton : MonoBehaviour
{
    [SerializeField] private float standardButton;  
    [SerializeField] private float pressedButton;
    
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var position = transform.localPosition;
            
            gameObject.transform.DOLocalMove(new Vector3(position.x, position.y, pressedButton), 0.25f)
                .SetEase(Ease.OutExpo)
                .OnComplete(() =>
                {
                    gameObject.transform.DOLocalMove(new Vector3(position.x, position.y, standardButton), 0.25f)
                        .SetEase(Ease.OutExpo);
                });

            GeneratorPuzzle.GeneratorEvents.CheckWarningLed();
        }
    }
}
