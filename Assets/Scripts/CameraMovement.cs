using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject moveToObject;

    public Vector3 startPostiion;
    public Vector3 startCameraPos;

    public bool track = true;
    
    void Start()
    {
        startPostiion = moveToObject.transform.position;
        startCameraPos = transform.position;
    }


    public void updateCamera()
    {
        if (track)
            transform.position = moveToObject.transform.position - startPostiion + startCameraPos;
    }
}
