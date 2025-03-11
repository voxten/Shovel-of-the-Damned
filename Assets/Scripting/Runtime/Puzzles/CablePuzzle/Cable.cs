using UnityEngine;

public class Cable : MonoBehaviour
{
    [SerializeField] private int segmentCount;
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private GameObject startPoint;
    [SerializeField] private GameObject endPoint;
    [SerializeField] private Color cableColor;
    [SerializeField] private GameObject cableParent;
    [SerializeField] private GameObject goodLamp;
    
    private GameObject[] segments;
    private bool haveCable;
    private Renderer startPointRenderer;
    private Renderer endPointRenderer;
    private Renderer segmentRenderer;
    private HingeJoint lastJoint;
    
    private Collider startCollider;
    private Collider endCollider;

    private void Awake()
    {
        startPointRenderer = startPoint.GetComponent<Renderer>();
        endPointRenderer = endPoint.GetComponent<Renderer>();
        startCollider = startPoint.GetComponent<Collider>();
        endCollider = endPoint.GetComponent<Collider>();
    }

    void Start()
    {
        startPointRenderer.material.color = cableColor;
        endPointRenderer.material.color = cableColor;
        
        segments = new GameObject[segmentCount];
        Vector3 segmentPosition = startPoint.transform.position;
        Vector3 direction = Vector3.down;

        for (int i = 0; i < segmentCount; i++)
        {
            segments[i] = Instantiate(segmentPrefab, segmentPosition, Quaternion.identity);
            segments[i].transform.parent = cableParent.transform;
            Rigidbody rb = segments[i].GetComponent<Rigidbody>();
            segmentRenderer = segments[i].GetComponent<Renderer>();
            segmentRenderer.material.color = cableColor;

            if (i > 0)
            {
                HingeJoint joint = segments[i].GetComponent<HingeJoint>();
                joint.connectedBody = segments[i - 1].GetComponent<Rigidbody>();
                joint.axis = Vector3.forward;
            }
            segmentPosition += direction*segments[i].transform.localScale.y;
        }

        HingeJoint firstJoint = segments[0].GetComponent<HingeJoint>();
        firstJoint.connectedBody = startPoint.GetComponent<Rigidbody>();

        segments[segmentCount - 1].transform.name = "Last";
    }

    public void OpenPuzzle()
    {
        startCollider.enabled = true;
        endCollider.enabled = true;
        foreach (Transform child in cableParent.transform)
        {
            child.GetComponent<Collider>().enabled = true;
        }
    }

    public void ClosePuzzle()
    {
        startCollider.enabled = true;
        endCollider.enabled = true;
        foreach (Transform child in cableParent.transform)
        {
            child.GetComponent<Collider>().enabled = false;
        }
    }
}
