using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour 
{
    [SerializeField] private float zoomIn = 30;
    [SerializeField] private float speedChangeRate = .5f;
    private float _defaultZoom;
    private float _zoom;

    [SerializeField] private List<CinemachineCamera> virtualCameras;
    
    private void Start() 
    {
        if (virtualCameras.Count == 0) return;
        _defaultZoom = virtualCameras[0].Lens.FieldOfView;
    }

    private void Update() 
    {
        if (Input.GetMouseButton(1)) 
        {
            _zoom = Mathf.Lerp(_zoom, _defaultZoom - zoomIn, Time.deltaTime * speedChangeRate);
            if(virtualCameras[0].Lens.FieldOfView > _defaultZoom - zoomIn + .1f)
                virtualCameras.ForEach(cameras => cameras.Lens.FieldOfView = _zoom);
        } 
        else 
        {
            _zoom = Mathf.Lerp(_zoom, _defaultZoom, Time.deltaTime * speedChangeRate);
            if(virtualCameras[0].Lens.FieldOfView < _defaultZoom - .1f)
                virtualCameras.ForEach(cameras => cameras.Lens.FieldOfView = _zoom);
        } 
    }
}
