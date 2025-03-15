using UnityEngine;

public class ItemCameraZoom : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private float zoomIn = 30;
    [SerializeField] private float speedChangeRate = .5f;
    private float _defaultZoom;
    private float _zoom;
    void Start()
    {
        _defaultZoom = camera.fieldOfView;
    }

    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            ZoomIn();
        }
        else
        {
            ZoomOut();
        }
    }

    private void ZoomIn()
    {
        _zoom = Mathf.Lerp(_zoom, _defaultZoom - zoomIn, Time.deltaTime * speedChangeRate);
        if (camera.fieldOfView > _defaultZoom - zoomIn + .1f && camera.fieldOfView > 1)
            camera.fieldOfView = _zoom;
    }

    private void ZoomOut()
    {
        _zoom = Mathf.Lerp(_zoom, _defaultZoom, Time.deltaTime * speedChangeRate);
        if (camera.fieldOfView < _defaultZoom - .1f)
            camera.fieldOfView = _zoom;
    }
}
