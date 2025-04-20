using UnityEngine;

public class SynchronizedLightPulse : MonoBehaviour
{
    [SerializeField] private Light spotLight; 
    [SerializeField] private Light pointLight; 

    [SerializeField] private float spotLightIntensity = 1.2f;
    [SerializeField] private float pointLightIntensity = 38f;

    [SerializeField] private float pulseSpeed = 1f;
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime * pulseSpeed;
        var t = 1f - Mathf.PingPong(_timer, 1f);
        
        if (spotLight != null)
            spotLight.intensity = Mathf.Lerp(0f, spotLightIntensity, t);

        if (pointLight != null)
            pointLight.intensity = Mathf.Lerp(0f, pointLightIntensity, t);
    }
}