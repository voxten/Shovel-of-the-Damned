using UnityEngine;

public class Pipe : MonoBehaviour
{
    
    [SerializeField] private PipeType pipeType;
    private Collider _collider;
    
    private readonly int[] _straightPossibleRotations = { 0, 90 };
    private readonly int[] _cornerPossibleRotations = { -90, 0, 180, 270 };
    
    private int _randomRotation;
    public float correctRotation;
    public bool canBeRotated ;
    private int _multipleRotation = 1;
    
    private void Start()
    {
        canBeRotated = true;
        if (gameObject.transform.rotation.eulerAngles.z != 270)
        {
            //correctRotation = gameObject.transform.rotation.eulerAngles.z;
        }
        else
        {
            //correctRotation = -90;
        }

        switch (pipeType)
        {
            case PipeType.Straight:
                _randomRotation = _straightPossibleRotations[Random.Range(0, _straightPossibleRotations.Length)];
                gameObject.transform.rotation = Quaternion.Euler(0, -90, _randomRotation);
                break;
            case PipeType.Corner:
                _randomRotation = _cornerPossibleRotations[Random.Range(0, _cornerPossibleRotations.Length)];
                gameObject.transform.rotation = Quaternion.Euler(0, -90, _randomRotation);
                break;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && canBeRotated)
        {
            SoundManager.PlaySound3D(PipePuzzle.PipePuzzleActions.GetPipeSound(), transform);
            if (pipeType == PipeType.Straight)
            {
                if (Mathf.Round(gameObject.transform.rotation.eulerAngles.z) == 90)
                {
                    gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
                }
                else
                {
                    gameObject.transform.rotation = Quaternion.Euler(0, -90, 90);
                }
            }
            else
            {
                gameObject.transform.rotation = Quaternion.Euler(0, -90, _multipleRotation * 90);
                _multipleRotation += 1;
                if (_multipleRotation == 5)
                {
                    _multipleRotation = 1;
                }
            }
            PipePuzzle.PipePuzzleActions.CheckRotations();
        }
    }
}


public enum PipeType
{
    Straight,
    Corner,
}