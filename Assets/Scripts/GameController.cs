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

    private const float SPAWN_DIST_MOVING = 0.7f;

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
    
    private float X_ROT_MIN = -12;
    private float X_ROT_MAX = 12;
    
    private float Y_ROT_MIN = -12;
    private float Y_ROT_MAX = 12;

    private int lives = 0;

    public GAME_STATE state;
    private float moveToSpawnTime;

    public CameraMovement camera;

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
        CreateLevel();
        CreateNextLevel();

        moveToSpawnTime = 0;
        state = GAME_STATE.MOVE_TO_SPAWN;
    }
    

    public void OnLevelEnd()
    {
        xRotation = yRotation = 0;
        _prevLevel = _currentLevel;
        _prevLevel.noJump = null;
        
        _currentLevel = _nextLevel;
        _currentLevel.noJump = _noJump;

        lives++;
        
        rigidBody.rotation = Quaternion.identity;
        rigidBody.angularVelocity = Vector3.zero;
        
        CreateNextLevel();
        state = GAME_STATE.MOVE_TO_SPAWN;
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

    void CreateLevel()
    {
        GameObject levelGO = Instantiate(LevelPrefab, sphere.transform.position + new Vector3(0,-100,0), Quaternion.identity, transform);
        _currentLevel = levelGO.GetComponent<Level>();
        _currentLevel.noJump = _noJump;
    }

    void CreateNextLevel()
    {
        GameObject levelGO = Instantiate(LevelPrefab, _currentLevel.NextLevelPos.position + new Vector3(0,-60,0), Quaternion.identity, transform);
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
    }
    
    void moveToSpawn()
    {
        rigidBody.velocity = (_currentLevel.spawn.position - sphere.transform.position) * 10;
        
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
        //camera.track = false;
        camera.transform.RotateAround(rotationPoint.transform.position,Vector3.forward, 2);

        if (dx !=0 || dy!=0 )
        {
            float changeX = dx * ROTATE_SPEED_X * Time.deltaTime;
            float changeY = dy * ROTATE_SPEED_Y * Time.deltaTime;


            if (xRotation + changeX > X_ROT_MIN && xRotation + changeX < X_ROT_MAX)
            {
                xRotation +=changeX;
                _currentLevel.rotateAround(rotateAround,  Vector3.right, changeX);
            }
           
            if (yRotation + changeY > Y_ROT_MIN && yRotation + changeY < Y_ROT_MAX)
            {
                yRotation +=changeY;
                _currentLevel.rotateAround(rotateAround, Vector3.forward, changeY);
            }


        }
        rigidBody.AddForce(Vector3.up, ForceMode.Force);
        
        Vector3 jumpDir = _currentLevel.g1.position - _currentLevel.g2.position;
        _noJump.jumpVector = jumpDir.normalized;
    }
    
}
