using System;
using System.Collections;
using StarterAssets;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioRepeat : MonoBehaviour {
    [SerializeField] private AudioClip[] stepsLeft;
    [SerializeField] private AudioClip[] stepsRight;
    [SerializeField] private float sprintDelay;
    [SerializeField] private float walkDelay;
    [SerializeField] private StarterAssetsInputs _input;

    private AudioSource audioSource;

    private bool _canMove = true;

    private void OnEnable() {
        FirstPersonController.PlayerEvents.ToggleMove += ToggleMove;
    }

    private void OnDisable() {
        FirstPersonController.PlayerEvents.ToggleMove -= ToggleMove;
    }

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        
    }

    private void Start() {
        StartCoroutine(PlayStep());
    }

    private void ToggleMove() {
        _canMove = !_canMove;
    }
    
    private void PlayRandomStep() {
        audioSource.PlayOneShot(stepsLeft[Random.Range(0,stepsLeft.Length)]);
    }

    private IEnumerator PlayStep() {
        while (true)
        {
            if (_input.move != Vector2.zero && _canMove) {
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                audioSource.PlayOneShot(stepsLeft[Random.Range(0,stepsLeft.Length)]);
            }

            if (_input.sprint)
            {
                yield return new WaitForSeconds(sprintDelay);
            }
            else if(_input.didAwake)
            {
                yield return new WaitForSeconds(walkDelay);
            }

            if (_input.move != Vector2.zero && _canMove) {
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                audioSource.PlayOneShot(stepsRight[Random.Range(0,stepsRight.Length)]);
            }

            if (_input.sprint)
            {
                yield return new WaitForSeconds(sprintDelay);
            }
            else if(_input.didAwake)
            {
                yield return new WaitForSeconds(walkDelay);
            }
        }
    }
}
