using System;
using System.Numerics;
using Unity.Cinemachine;
using StarterAssets;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class HeadBob : MonoBehaviour {
    [SerializeField] private CinemachineCamera cameraRun;
    [SerializeField] private CinemachineCamera cameraWalk;
    [SerializeField] private CharacterController character; // Add the rigidbody of your Player to your script
    [SerializeField] private FirstPersonController firstPersonController;
    private CinemachineBasicMultiChannelPerlin cam1;
    [SerializeField] private Vector3 vel;
    [SerializeField] private NoiseSettings idle;
    [SerializeField] private NoiseSettings walk;
    private StarterAssetsInputs _input;

    private void Start() {
        _input = GetComponent<StarterAssetsInputs>();
        firstPersonController = GetComponent<FirstPersonController>();
    }

    void Update() {
        vel = transform.rotation * character.velocity;

        if (_input.move != Vector2.zero) {
            if (_input.sprint) {
                cameraWalk.Priority = 9;
                cameraRun.Priority = 11;
            }
            else {
                cameraWalk.Priority = 11;
                cameraRun.Priority = 9;    
            }
        }
        else {
            cameraWalk.Priority = 9;
            cameraRun.Priority = 9;
        }

        // if(vel.z > .5f && vel.z < 5) {
        //     // forward
        //     cameraWalk.Priority = 11;
        //     cameraRun.Priority = 9;
        // } else if(vel.z < -.5f && vel.z > -5) {
        //     cameraRun.Priority = 9;
        //     cameraWalk.Priority = 11;
        // } else if (vel.z > 5) {
        //     cameraWalk.Priority = 9;
        //     cameraRun.Priority = 11;
        // } else if (vel.z < -5) {
        //     cameraWalk.Priority = 9;
        //     cameraRun.Priority = 11;
        // }
        // else {
        //     cameraWalk.Priority = 9;
        //     cameraRun.Priority = 9;
        // }
        // if(vel.x > 0) {
        //     // right
        // } else if(vel.x < 0) {
        //     // left
        // }
    }
}
