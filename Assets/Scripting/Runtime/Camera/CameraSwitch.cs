using UnityEngine;
using Unity.Cinemachine;
using System;
using System.Collections;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private CinemachineCamera defaultCamera;
    [SerializeField] private CinemachineBrain cinemachineBrain;

    private CinemachineCamera _activeCamera;
    
    private void OnEnable()
    {
        CameraEvents.SwitchCamera += SwitchCamera;
        CameraEvents.SwitchToDefault += SwitchToDefault;
        CameraEvents.SwitchCameraToDefaultWithTime += SwitchCameraToDefaultWithTime;
        _activeCamera = defaultCamera;
    }

    private void OnDisable()
    {
        CameraEvents.SwitchCamera -= SwitchCamera;
        CameraEvents.SwitchToDefault -= SwitchToDefault;
        CameraEvents.SwitchCameraToDefaultWithTime -= SwitchCameraToDefaultWithTime;
        _activeCamera = null;
    }

    private void SwitchCamera(CinemachineCamera virtualCamera)
    {
        virtualCamera.Priority = 12;    
        _activeCamera.Priority = 9;
        
        if(cinemachineBrain.enabled == false) 
        {
            cinemachineBrain.enabled = true;
        }
        
        _activeCamera = virtualCamera;
    }

    private void SwitchCameraToDefaultWithTime(float time)
    {
        StartCoroutine(WaitForCameraSwitch(time));
    }

    private void SwitchToDefault()
    {
        SwitchCamera(defaultCamera);
    }

    private IEnumerator WaitForCameraSwitch(float time)
    {
        yield return new WaitForSeconds(time);
        SwitchToDefault();
    }
    
    public static class CameraEvents
    {
        public static Action<CinemachineCamera> SwitchCamera;
        public static Action SwitchToDefault;
        public static Action<float> SwitchCameraToDefaultWithTime;
    }
}