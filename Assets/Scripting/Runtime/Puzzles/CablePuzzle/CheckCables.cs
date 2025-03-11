using UnityEngine;

public class CheckCables : MonoBehaviour
{
    [SerializeField] private GameObject[] endPoints;
    [SerializeField] private GameObject[] goodLamps;
    private Renderer endPointRenderer;
    private HingeJoint endPointHingeJoint;
    private Renderer cableRenderer;
    private Renderer goodLampRenderer;
    private bool isGood;

    private void OnMouseOver()
    {
        isGood = true;
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < endPoints.Length; i++)
            {
                endPointHingeJoint = endPoints[i].GetComponent<HingeJoint>();
                endPointRenderer = endPoints[i].GetComponent<Renderer>();
                if (endPointHingeJoint.connectedBody != null)
                {
                    cableRenderer = endPointHingeJoint.connectedBody.GetComponent<Renderer>();
                    if (endPointRenderer.material.color != cableRenderer.material.color)
                    {
                        isGood = false;
                    }
                    else
                    {
                        goodLampRenderer = goodLamps[i].gameObject.GetComponent<Renderer>();
                        goodLampRenderer.material.color = Color.yellow;
                    }
                }
                else
                {
                    isGood = false;
                }
            }

            if (isGood) Debug.Log("WYGRLES");
        }
    }
}
