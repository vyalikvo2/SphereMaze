using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject moveToObject;
    
    public Quaternion startLook;
    public Quaternion curLook;
    public float startCameraDist;
    public bool track = true;
    
    void Start()
    {
        
        startLook = Quaternion.LookRotation(moveToObject.transform.position - transform.position);
        curLook = startLook;
        
        startCameraDist = (transform.position - moveToObject.transform.position).magnitude;
        
    }


    public void updateCamera()
    {
        if (track)
        {
            Quaternion q = curLook;
            Vector3 dir = - (new Vector3(2 * (q.x * q.z + q.w * q.y), 2 * (q.y * q.z - q.w * q.x), 1 - 2 * (q.x * q.x + q.y * q.y)));
            dir.Scale(new Vector3(startCameraDist, startCameraDist, startCameraDist));

            transform.position = moveToObject.transform.position + dir;
            transform.rotation = q;
        }
            //transform.position = moveToObject.transform.position - startPostiion + startCameraPos;*/
    }
}
