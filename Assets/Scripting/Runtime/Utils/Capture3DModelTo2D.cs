using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;

public class Capture3DModelTo2D : MonoBehaviour
{
    [SerializeField] private GameObject model; // Assign your 3D model here
    [SerializeField] private float cameraDistance = 5f; // Distance from the model
    [SerializeField] private int imageWidth = 512;
    [SerializeField] private int imageHeight = 512;
    [SerializeField] private bool transparentBackground = true; // Enable transparency
    private Camera _renderCamera;

    [Button]
    private void Capture()
    {
        if (model == null)
        {
            Debug.LogError("No model assigned!");
            return;
        }

        // Create a new Camera
        GameObject camObj = new GameObject("RenderCamera");
        _renderCamera = camObj.AddComponent<Camera>();

        // Camera Settings
        _renderCamera.clearFlags = transparentBackground ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
        _renderCamera.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : Color.white;
        _renderCamera.orthographic = false;
        _renderCamera.fieldOfView = 60;

        // Position Camera at the set distance
        Bounds modelBounds = GetModelBounds(model);
        Vector3 modelCenter = modelBounds.center;
        float adjustedDistance = modelBounds.extents.magnitude + cameraDistance;
        _renderCamera.transform.position = modelCenter + new Vector3(0, 0, -adjustedDistance);
        _renderCamera.transform.LookAt(modelCenter);

        // Create a RenderTexture
        RenderTexture renderTex = new RenderTexture(imageWidth, imageHeight, 32);
        _renderCamera.targetTexture = renderTex;

        // Capture and save
        StartCoroutine(CaptureImage(renderTex));
    }

    private System.Collections.IEnumerator CaptureImage(RenderTexture renderTex)
    {
        yield return new WaitForEndOfFrame();

        // Set active RenderTexture
        RenderTexture.active = renderTex;

        // Create Texture2D and read pixels
        TextureFormat format = transparentBackground ? TextureFormat.RGBA32 : TextureFormat.RGB24;
        Texture2D tex = new Texture2D(imageWidth, imageHeight, format, false);
        tex.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        tex.Apply();

        // Save as PNG
        byte[] bytes = tex.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, model.name + ".png");
        File.WriteAllBytes(path, bytes);

        Debug.Log("Screenshot saved to: " + path);

        // Clean up
        RenderTexture.active = null;
        _renderCamera.targetTexture = null;
        Destroy(renderTex);
        Destroy(_renderCamera.gameObject);
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
