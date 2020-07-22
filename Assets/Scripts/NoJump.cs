using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoJump : MonoBehaviour
{
    private bool noJumpZone = false;
    public Vector3 jumpVector;

    private Rigidbody _rigidbody;

    public GameController gc;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        jumpVector = new Vector3(0, 1, 0);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "NoJump")
        {
            Debug.Log("Enter no jump");
            noJumpZone = true;
        }
        
        if (collider.gameObject.tag == "EndLevel")
        {
            gc.OnLevelEnd();
            noJumpZone = false;
        }
        
        if (collider.gameObject.tag == "Dead")
        {
            gc.OnDeadHit();
        }
    }
    
    void OnTriggerLeave(Collider collider)
    {
        if (collider.gameObject.tag == "NoJump")
        {
            noJumpZone = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gc.state != GAME_STATE.GAME) return;
        if (noJumpZone)
        {
            _rigidbody.AddForce(-jumpVector * 60, ForceMode.Acceleration);
        }
    }
}
