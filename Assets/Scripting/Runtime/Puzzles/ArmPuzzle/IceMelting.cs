using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class IceMelting : MonoBehaviour
{
    [SerializeField] private float meltingSpeed = 0.1f;
    //[SerializeField] private GameObject puddle;
    [SerializeField] private ParticleSystem waterDripEffect;
    //[SerializeField] private Vector3 puddleMaxSize = new(0.5f, 1f, 0.5f);
    [SerializeField] private Animator handAnimator;
    [SerializeField] private GameObject key;
    [SerializeField] private BunsenBurnerTank bunsenBurnerTank;
    [SerializeField] private Sound meltSound;
    
    private Material _iceMaterial;
    private Vector3 _initialScale;
    private float _meltProgress;
    private bool _isMelting;

    private void Start()
    {
        _iceMaterial = GetComponent<Renderer>().material;
        _initialScale = transform.localScale;
        if (waterDripEffect) 
            waterDripEffect.Stop();
    }

    private void OnDestroy()
    {
        handAnimator.SetTrigger("Melt");
        key.transform.SetParent(null);
        key.AddComponent<Rigidbody>();
        key.GetComponent<Rigidbody>().mass = 150;
        key.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        key.GetComponent<PickItem>().isPickable = true;
    }

    private void Update()
    {
        if (_isMelting)
        {
            MeltIce();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ice"))
        {
            if(!bunsenBurnerTank.isOn)
                Narration.DisplayText?.Invoke("There's no propane.");
            else 
                SoundManager.PlaySound(meltSound);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ice"))
        {
            if(bunsenBurnerTank.isOn)
                StartMelting();
            Debug.Log("Ice triggered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ice"))
        {
            if (bunsenBurnerTank.isOn)
            {
                _isMelting = false;
                SoundManager.StopSound(meltSound);
            }
                
            Debug.Log("Ice not triggered");
        }
    }

    [Button]
    public void StartMelting()
    {
        _isMelting = true;
        if (waterDripEffect) 
            waterDripEffect.Play();
    }

    private void MeltIce()
    {
        // Reduce the scale over time
        transform.localScale = Vector3.Lerp(_initialScale, Vector3.zero, _meltProgress);

        // Fade transparency using shader property (assuming material supports transparency)
        Color color = _iceMaterial.color;
        color.a = Mathf.Lerp(1, 0, _meltProgress);
        _iceMaterial.color = color;

        // Increase melt progress
        _meltProgress += Time.deltaTime * meltingSpeed;

        /* Increase puddle size based on user-defined max size
        if (puddle)
        {
            puddle.transform.localScale = Vector3.Lerp(Vector3.zero, puddleMaxSize, _meltProgress);
        }
        */
        // Stop melting when fully melted
        if (_meltProgress >= 1f)
        {
            if (waterDripEffect) 
                waterDripEffect.Stop();
            Destroy(gameObject);
        }
    }
}