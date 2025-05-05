using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BunsenBurnerTank : InteractableObject
{
    [SerializeField] private GameObject burner;
    [SerializeField] private GameObject flameParticles;
    private bool _isFirst = true;
    public bool isOn;

    private void Start()
    {
        flameParticles.SetActive(false);
    }

    public override bool Interact()
    {
        if (_isFirst)
        {
            _isFirst = false;
            StartCoroutine(WaitForBurner());
        }
        return true;
    }

    private IEnumerator WaitForBurner()
    {
        SoundManager.PlaySound3D(Sound.BunsenStart, transform);
        yield return new WaitForSeconds(0.5f);
        if (isOn)
        {
            StopFire();
        }
        else
        {
            StartFire();
        }
        _isFirst = true;
    }

    private void StartFire()
    {
        isOn = true;
        flameParticles.SetActive(true);
        
        SoundManager.PlaySound3D(Sound.BunsenLoop, burner.transform);
    }

    private void StopFire()
    {
        isOn = false;
        SoundManager.StopSound(Sound.BunsenLoop);
        flameParticles.SetActive(false);
    }
}
