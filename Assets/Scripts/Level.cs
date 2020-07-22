using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Transform spawn;

    // gravity
    public Transform g1;
    public Transform g2;
    public Transform c2;
    public Transform y2;
    
    public MeshRenderer ramp1;
    public MeshRenderer ramp2;

    private List<MeshRenderer> levelMeshes;

    private Vector3 jumpDir;

    public Transform NextLevelPos;
    public NoJump noJump;
    
    // Start is called before the first frame update
    void Start()
    {
        levelMeshes = new List<MeshRenderer>();
        levelMeshes.Add(ramp1);
        levelMeshes.Add(ramp2);
    }

    public void rotateAround(Vector3 point, Vector3 axis, float power)
    {
        for (int i = 0; i < levelMeshes.Count; i++)
        {
            levelMeshes[i].transform.RotateAround(point,axis,power);
        }
    }
}
