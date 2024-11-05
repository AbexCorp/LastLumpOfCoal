using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance;

    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private Camera _camera;

    [SerializeField]
    private float _moveSpeed = 1;
    private Vector2 _movement;


    private bool _IsInsideTrain = false;
    private bool _isDrivingTrain = false;
    public bool IsDrivingTrain => _isDrivingTrain;


    [SerializeField]
    private GameObject _light;



    private void Awake()
    {
        Instance = this;
        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _camera = Camera.main;
    }
    void Start()
    {
        _IsInsideTrain = true;
        _isDrivingTrain = true;
        SnapPlayerToTrain();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        Train.Instance.OnDoorClose += TurnOffLight;
        Train.Instance.OnDoorOpen += TurnOnLight;
    }

    
    void Update()
    {
        _rigidBody.velocity = _movement * _moveSpeed;
        if (_isDrivingTrain)
        {
            _camera.transform.position = new Vector3(Train.Instance.TrainPosition.x, 0, -10);
            _camera.orthographicSize = 7;
        }
        else
        {
            _camera.transform.position = new Vector3(_rigidBody.position.x, _rigidBody.position.y, -10);
            _camera.orthographicSize = 3.6f;

        }
        if (!_IsInsideTrain && !_isDrivingTrain)
        {
            if(Vector2.Distance(_rigidBody.transform.position, Train.Instance.TrainPosition) > Danger.Instance.CurrentRadius + 0.3f)
            Train.Instance.LoseGameBugAteYou();
        }
    }
    public void PlayerEnteredTrain()
    {
        _IsInsideTrain = true;
    }
    public void PlayerLeftTrain()
    {
        _IsInsideTrain = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        
    }
    public void OperateTrain()
    {
        if (!_IsInsideTrain)
            return;

        if (!_isDrivingTrain)
        {
            _isDrivingTrain = true;
            SnapPlayerToTrain();
            _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
            Train.Instance.StartDrivingTrain();
        }
        else if(_isDrivingTrain)
        {
            _isDrivingTrain = false;
            _rigidBody.constraints = RigidbodyConstraints2D.None;
            _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    public void SnapPlayerToTrain()
    {
        transform.position = Train.Instance.TrainPosition;
        Physics2D.SyncTransforms();
    }

    public void TurnOnLight()
    {
        _light.SetActive(true);
    }
    public void TurnOffLight()
    {
        _light.SetActive(false);
    }
}
