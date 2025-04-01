using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using System.Collections;

public class Capture3DModelTo2D : MonoBehaviour
{
    [SerializeField] private GameObject model; // Assign your 3D model here
    [SerializeField] private float cameraDistance = 5f; // Distance from the model
    [SerializeField] private int imageWidth = 512;
    [SerializeField] private int imageHeight = 512;
    [SerializeField] private bool transparentBackground = true; // Enable transparency
    private Camera _renderCamera;
    private RenderTexture _renderTexture;

    private void Awake()
    {
        SetupCamera();
    }

    private void SetupCamera()
    {
        if (_renderCamera == null)
        {
            GameObject camObj = new GameObject("RenderCamera");
            _renderCamera = camObj.AddComponent<Camera>();
            _renderCamera.transform.SetParent(transform); // Keep it in the hierarchy
        }

        _renderCamera.clearFlags = transparentBackground ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
        _renderCamera.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : Color.white;
        _renderCamera.orthographic = false;
        _renderCamera.fieldOfView = 60;
        _renderCamera.enabled = false; // Disable until capture

        _renderTexture = new RenderTexture(imageWidth, imageHeight, 32);
        _renderCamera.targetTexture = _renderTexture;
    }

    [Button]
    private void Capture()
    {
        if (model == null)
        {
            Debug.LogError("No model assigned!");
            return;
        }

        // Get model bounds
        Bounds modelBounds = GetModelBounds(model);
        Vector3 modelCenter = modelBounds.center;
        float adjustedDistance = modelBounds.extents.magnitude + cameraDistance;

        // Position camera
        _renderCamera.transform.position = modelCenter + new Vector3(0, 0, -adjustedDistance);
        _renderCamera.transform.LookAt(modelCenter);

        // Capture and save image
        StartCoroutine(CaptureImage());
    }

    private IEnumerator CaptureImage()
    {
        yield return new WaitForEndOfFrame();

        // Set active RenderTexture
        RenderTexture.active = _renderTexture;
        _renderCamera.Render(); // Manually render the camera

        // Read pixels
        TextureFormat format = transparentBackground ? TextureFormat.RGBA32 : TextureFormat.RGB24;
        Texture2D tex = new Texture2D(imageWidth, imageHeight, format, false);
        tex.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        tex.Apply();

        // Save as PNG
        string directory = Path.Combine(Application.dataPath, "Art/2D");
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string path = Path.Combine(directory, model.name + ".png");
        File.WriteAllBytes(path, tex.EncodeToPNG());

        Debug.Log("Screenshot saved to: " + path);

        // Cleanup
        RenderTexture.active = null;
        Destroy(tex);
    }

    // Calculate the bounds of the model
    private Bounds GetModelBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }
        return bounds;
    }
}
