using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_STATE
{
    MOVE_TO_SPAWN,
    GAME,
    GAME_OVER,
}

public class GameController : MonoBehaviour
{
    private const float ROTATE_SPEED_X = 35;
    private const float ROTATE_SPEED_Y = 35;

    private const float SPAWN_DIST_MOVING = 1.5f;

    public GameObject sphere;
    public Rigidbody rigidBody;
    public GameObject rotationPoint;

    public GameObject LevelPrefab;
    
    private NoJump _noJump;
    
    private Level _prevLevel;
    private Level _currentLevel;
    private Level _nextLevel;
    
    private float xRotation = 0;
    private float yRotation = 0;
    
    private float X_ROT_MIN = -18;
    private float X_ROT_MAX = 18;
    
    private float Y_ROT_MIN = -13;
    private float Y_ROT_MAX = 13;

    private int lives = 0;

    public GAME_STATE state;
    private float moveToSpawnTime;

    public CameraMovement camera;

    private Quaternion startLevelDirRot;
    private float plusedRot = 0;

    public void Reset()
    {
        if (_prevLevel)
        {
            Destroy(_prevLevel.gameObject);
        }
        if (_currentLevel)
        {
            Destroy(_currentLevel.gameObject);
        }
        if (_nextLevel)
        {
            Destroy(_nextLevel.gameObject);
        }

        xRotation = yRotation = 0;
        rigidBody.velocity = Vector3.zero;
        rigidBody.rotation = Quaternion.identity;
        rigidBody.angularVelocity = Vector3.zero;
        sphere.transform.position = Vector3.zero;

        camera.curLook = camera.startLook;
        camera.updateCamera();
        plusedRot = 0;

        init();

        camera.track = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, -120.0f, 0);

        rigidBody = sphere.GetComponent<Rigidbody>();
        _noJump = sphere.GetComponent<NoJump>();

        _noJump.gc = this;

        init();
    }

    void init()
    {
        CreateStartLevel();
        moveToSpawnTime = 0;
        state = GAME_STATE.MOVE_TO_SPAWN;
        _noJump.ResetLevel();
    }
    

    public void OnLevelEnd()
    {
        xRotation = yRotation = 0;
        _prevLevel = _currentLevel;
        _prevLevel.noJump = null;
        
        CreateNextLevel();
        _noJump.ResetLevel();
        _currentLevel = _nextLevel;
        _currentLevel.noJump = _noJump;

        lives++;
        
        rigidBody.rotation = Quaternion.identity;
        rigidBody.angularVelocity = Vector3.zero;
        
        
        state = GAME_STATE.MOVE_TO_SPAWN;
    }

    public void ResetCamera()
    {
        if (state != GAME_STATE.GAME_OVER)
        {
            camera.curLook = camera.startLook;
            camera.updateCamera();
            plusedRot = 0;
        }
    }

    void onSpawn()
    {
        if (_prevLevel != null)
        {
            Debug.Log(" Destroy " + _prevLevel);
            Destroy(_prevLevel.gameObject);
            _prevLevel = null;
        }

        lives = 0;
    }

    void CreateStartLevel()
    {
        GameObject levelGO = Instantiate(LevelPrefab, sphere.transform.position + new Vector3(0,0,0), Quaternion.identity, transform);
        _currentLevel = levelGO.GetComponent<Level>();
        _currentLevel.noJump = _noJump;

        startLevelDirRot = Quaternion.LookRotation(_currentLevel.c2.position - _currentLevel.g2.position);
    }

    void CreateNextLevel()
    {
        GameObject levelGO = Instantiate(LevelPrefab, sphere.transform.position + new Vector3(0,-5,0), Quaternion.identity, transform);
        _nextLevel = levelGO.GetComponent<Level>();
    }

    public void OnDeadHit()
    {
        lives--;
        if (lives < 0)
        {
            state = GAME_STATE.GAME_OVER;
            camera.track = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        mainUpdateCamera();
        switch (state)
        {
            case GAME_STATE.GAME:
                gameState();
                break;
            case GAME_STATE.MOVE_TO_SPAWN:
                moveToSpawn();
                break;
        }
    }

    void mainUpdateCamera()
    {
        camera.updateCamera();
        
        Quaternion curlevelRot = Quaternion.LookRotation(_currentLevel.c2.position - _currentLevel.g2.position);
        float rotateChange = curlevelRot.eulerAngles.y - startLevelDirRot.eulerAngles.y + plusedRot;
        
        camera.transform.RotateAround(sphere.transform.position, Vector3.up, rotateChange);
        plusedRot -= rotateChange;
        
        camera.curLook = Quaternion.LookRotation(sphere.transform.position - camera.transform.position);
    }
    
    void moveToSpawn()
    {
        //rigidBody.velocity = (_currentLevel.spawn.position - sphere.transform.position) * 10;
        rigidBody.velocity = new Vector3( 0, rigidBody.velocity.y, 0);
        if ((sphere.transform.position - _currentLevel.spawn.position).sqrMagnitude < SPAWN_DIST_MOVING*SPAWN_DIST_MOVING )
        {
            state = GAME_STATE.GAME; // spawned
            onSpawn();
        }
    }

    void gameState()
    {
        if (rigidBody == null) return;
        
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        
        Vector3 rotateAround = rotationPoint.transform.position;

        if (dx !=0 || dy!=0 )
        {
            float changeX = dx * ROTATE_SPEED_X * Time.deltaTime;
            float changeY = dy * ROTATE_SPEED_Y * Time.deltaTime;
            
            if (xRotation + changeX > X_ROT_MIN && xRotation + changeX < X_ROT_MAX)
            {
                xRotation +=changeX;
                Vector3 xAxis = (_currentLevel.g2.position-_currentLevel.y2.position).normalized;
                //_currentLevel.rotateAround(rotateAround,  Vector3.right, changeX);
                _currentLevel.rotateAround(rotateAround,  xAxis, changeX);
            }
           
            if (yRotation + changeY > Y_ROT_MIN && yRotation + changeY < Y_ROT_MAX)
            {
                yRotation +=changeY;
                Vector3 yAxis = (_currentLevel.c2.position - _currentLevel.g2.position).normalized;
                //_currentLevel.rotateAround(rotateAround, Vector3.forward, changeY);
                _currentLevel.rotateAround(rotateAround, yAxis, changeY);
            }


        }
        rigidBody.AddForce(Vector3.up, ForceMode.Force);
        
        Vector3 jumpDir = _currentLevel.g1.position - _currentLevel.g2.position;
        _noJump.jumpVector = jumpDir.normalized;
    }
    
}
