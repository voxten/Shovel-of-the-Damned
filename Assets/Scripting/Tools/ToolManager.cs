using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    
    [SerializeField] private Sound pickSound;
    [SerializeField] private Transform _toolHolder;
    [SerializeField] private float _toolThrowForce = 5f;
    
    [ShowInInspector, ReadOnly] public Tool CurrentTool { get; private set; }

    private bool _isTweeningInProgress;
    
    public static ToolManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !InteractionSystem.InteractionEvents.GetInteraction())
        {
            TryDropTool();
        }
    }

    public void TryPickupTool(Tool tool)
    {
        if (CurrentTool is not null)
        {
            Narration.DisplayText?.Invoke("My hand is occupied, I can't pick it up...");
            Debug.LogWarning($"Already holding a tool: {CurrentTool.name}");
            return;
        }
        
        CurrentTool = tool;
        SoundManager.PlaySound3D(pickSound,tool.transform);
        // Debug.Log($"Picked up tool: {tool.name}");
        
        var toolRigidbody = tool.GetComponent<Rigidbody>();
        
        toolRigidbody.isKinematic = true;
        tool.DisableColliders();
        
        SetGameLayerRecursive(tool.gameObject, LayerMask.NameToLayer("RenderInFront"));
        
        _isTweeningInProgress = true;
        
        tool.transform.SetParent(_toolHolder);
        tool.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutExpo);
        tool.transform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutExpo).onComplete += () => _isTweeningInProgress = false;
    }
    
    private void TryDropTool()
    {
        if (CurrentTool is null)
        {
            // Debug.LogWarning("No tool to drop");
            return;
        }
        
        if (_isTweeningInProgress)
        {
            // Debug.LogWarning("Cannot drop tool while tweening");
            return;
        }
        
        var droppedTool = CurrentTool;
        CurrentTool = null;
        
        // Debug.Log($"Dropped tool: {droppedTool.name}");
        
        droppedTool.transform.SetParent(null);
        var droppedToolRigidbody = droppedTool.GetComponent<Rigidbody>();
        
        droppedTool.transform.position = _cameraTransform.position;
        
        droppedToolRigidbody.isKinematic = false;
        droppedToolRigidbody.AddForce(Camera.main.transform.forward * _toolThrowForce, ForceMode.Impulse);
        droppedToolRigidbody.angularVelocity = UnityEngine.Random.insideUnitSphere * 4f;

        StartCoroutine(EnableCollidersDelayed(droppedTool, 0.05f));
        
        SetGameLayerRecursive(droppedTool.gameObject, LayerMask.NameToLayer("Interaction"));
    }
    
    private IEnumerator EnableCollidersDelayed(Tool tool, float delay)
    {
        yield return new WaitForSeconds(delay);
        tool.EnableColliders();
    }
    
    private void SetGameLayerRecursive(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.layer = layer;

            Transform children = child.GetComponentInChildren<Transform>();
            if (children != null)
                SetGameLayerRecursive(child.gameObject, layer);
              
        }
    }

    public void RemoveTool()
    {
        CurrentTool.gameObject.SetActive(false);
        CurrentTool = null;
    }
    
    public void ResetTool() {
        SetGameLayerRecursive(CurrentTool.gameObject, LayerMask.NameToLayer("Interaction"));
        CurrentTool.transform.SetParent(null);
        CurrentTool.EnableColliders();
        CurrentTool = null;
    }
    
    public bool CompareToolType(ToolType toolType)
    {
        return CurrentTool?.toolType == toolType;
    }

    public void ToggleTool()
    {
        _toolHolder.gameObject.SetActive(!_toolHolder.gameObject.activeInHierarchy);
    }
}
