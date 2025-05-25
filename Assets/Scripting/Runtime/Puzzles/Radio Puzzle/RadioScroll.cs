using UnityEngine;

public class RadioScroll : MonoBehaviour
{
    [SerializeField] private RadioNeedle needle; // Reference to the needle
    private bool _isRotating;
    private float _currentPos;
    private RadioPuzzle radioPuzzle;


    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!RadioPuzzle.RadioEvents.GetIsFinished())
                _isRotating = true;
        }
    }
    private void Start()
    {
        radioPuzzle = FindObjectOfType<RadioPuzzle>();
        if (radioPuzzle.isAfterLoad)
        {
            if (radioPuzzle.isFinished)
            {
                needle.transform.localPosition = new Vector3(needle.transform.localPosition.x, needle.transform.localPosition.y, needle.correctPos);
                RadioPuzzle.RadioEvents.CheckNeedles();
            }
        }

    }

    private void Update()
    {
        if (_isRotating)
        {
            if (RadioPuzzle.RadioEvents.GetIsFinished())
            {
                _isRotating = false;
                return;
            }
            
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                SoundManager.PlaySound3D(Sound.RadioTuner, transform, null, 0.6f);
                transform.Rotate(0f, -scrollInput * 1000f, 0f);
                
                float newPos = needle.transform.localPosition.z + scrollInput * 0.05f;
                
                newPos = Mathf.Clamp(newPos, needle.minPos, needle.maxPos);
                
                if (Mathf.Approximately(needle.transform.localPosition.z, needle.minPos) && scrollInput < 0)
                {
                    newPos = needle.minPos;
                }

                _currentPos = newPos;
                
                needle.transform.localPosition = new Vector3(needle.transform.localPosition.x, needle.transform.localPosition.y, newPos);

                RadioPuzzle.RadioEvents.CheckNeedles();
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isRotating = false;
            }
        }
        
    }

    public bool CheckNeedlePosition()
    {
        if (Mathf.Approximately(needle.correctPos, _currentPos))
        {
            return true;
        }
        return false;
    }
}