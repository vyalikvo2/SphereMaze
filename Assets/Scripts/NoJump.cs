using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoJump : MonoBehaviour
{
    private Rigidbody _rigidbody;

    public GameController gc;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void ResetLevel()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "EndLevel")
        {
            gc.OnLevelEnd();
        }
        
        if (collider.gameObject.tag == "Dead")
        {
            gc.OnDeadHit();
        }
        if (collider.gameObject.tag == "ResetCamera")
        {
            gc.ResetCamera();
        }
    }
}
