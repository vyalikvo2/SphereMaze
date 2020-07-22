using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    private Touch touch;
    private float touchSpeed = 2.0f;
    private Vector2 touchMove;

    private GameController gc;

    private Vector2 touchStartPos;
    private Vector2 touchCurPos;

    // Start is called before the first frame update
    void Start()
    {
        touchMove = Vector2.zero;
        gc = GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                touchCurPos = touchStartPos;
            }
            
            touchCurPos = touch.position;
            Debug.Log(touchCurPos - touchStartPos);
                //gc.rotateGravity(touchCurPos - touchStartPos, Time.deltaTime/100.0f);
               
        }
        //gc.rotateInGameInput(new Vector2(1,0), Time.deltaTime/10.0f);
    }
}
