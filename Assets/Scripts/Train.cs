using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Train : MonoBehaviour
{
    public static Train Instance;
    private Rigidbody2D _rigidBody;

    [SerializeField]
    private GameObject _driveLights;
    [SerializeField]
    private GameObject _stopLights;

    public Vector2 TrainPosition => transform.position;

    private bool _drive = false;
    private bool _isTravelling = false;
    private bool _brake = false;

    public int DistanceTravelled {  get; private set; }
    private bool _isLosingGame = false;

    private void Awake()
    {
        Instance = this;
        _rigidBody = GetComponent<Rigidbody2D>();
        DistanceTravelled = 0;
    }


    void Start()
    {
        MenuManager.Instance.UpdateDistanceTravelled();
        BugManager.Instance.SpawnBugs();
    }
    private void Update()
    {
        if (_drive)
        {
            if (_rigidBody.velocity.x < 10)
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x + (2f * Time.deltaTime), _rigidBody.velocity.y);
            else if(!_isTravelling)
            {
                _isTravelling = true;
                StartCoroutine(Travel());
            }
            Player.Instance.SnapPlayerToTrain();
        }
        else if(_brake)
        {
            if(_rigidBody.velocity.x < 0.1f)
                EndBraking();
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x - (1.5f * Time.deltaTime), _rigidBody.velocity.y);
            Player.Instance.SnapPlayerToTrain();
        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Application.isPlaying || MenuManager.Instance == null || Player.Instance == null)
            return;
        if (Player.Instance.IsDrivingTrain)
            return;
        MenuManager.Instance.ShowTrainMenu(doorsOpen: true);
        Player.Instance.PlayerEnteredTrain();
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!Application.isPlaying || MenuManager.Instance == null || Player.Instance == null)
            return;
        MenuManager.Instance.HideTrainMenu();
        Player.Instance.PlayerLeftTrain();
    }




    public event Action OnDoorOpen;
    public void OpenDoor()
    {
        Player.Instance.OperateTrain();
        MenuManager.Instance.LockSliders();
        OnDoorOpen?.Invoke();
        _stopLights.SetActive(true);
    }
    public event Action OnDoorClose;
    public void CloseDoor()
    {
        Resources.Instance.CullRemainingEmployees();
        if(Resources.Instance.NumberOfEmployees == 0)
        {
            LoseGameOutOfEmployees();
            return;
        }
        if(Resources.Instance.Coal <= 0)
        {
            LoseGameRunOutOfCoalStationary();
            return;
        }
        Player.Instance.OperateTrain();
        MenuManager.Instance.UnlockSliders();
        OnDoorClose?.Invoke();
        BugManager.Instance.CullBugs();
        _stopLights.SetActive(false);
    }


    public void StartDrivingTrain()
    {
        _drive = true;
        _rigidBody.constraints = RigidbodyConstraints2D.None;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        _driveLights.SetActive(true);
    }
    public void StartBrakingTrain()
    {
        _drive = false;
        _isTravelling = false;
        _brake = true;
    }
    private void EndBraking()
    {
        _driveLights.SetActive(false);
        _brake = false;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        Danger.Instance.PrepNewMap();
        MenuManager.Instance.ShowTrainMenu(doorsOpen: false);
    }
    private IEnumerator Travel()
    {
        YieldInstruction yield = new WaitForSeconds(1.85f);
        while(true)
        {
            yield return yield;
            DistanceTravelled += 100;
            MenuManager.Instance.UpdateDistanceTravelled();
            if (DistanceTravelled % 200 == 0)
                Resources.Instance.DepleteCoal(1);
            if (DistanceTravelled % 500 == 0 && DistanceTravelled % 1000 != 0)
                Resources.Instance.DepleteFood(Resources.Instance.NumberOfEmployees + 1);
            if (DistanceTravelled % 1000 == 0)
            {
                StartBrakingTrain();
                break;
            }
        }
    }




    public void LoseGameRunOutOfCoalDriving()
    {
        if (_isLosingGame)
            return;
        _isLosingGame = true;
        _drive = false;
        StopAllCoroutines();
        StartCoroutine(StopTrainDeath());
    }
    private IEnumerator StopTrainDeath()
    {
        while(_rigidBody.velocity.x > 0f)
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x - (1.5f * Time.deltaTime), _rigidBody.velocity.y);
            Player.Instance.SnapPlayerToTrain();
            if(_rigidBody.velocity.x <= 0.1f)
            {
                _rigidBody.velocity = Vector2.zero;
                _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            }
            yield return null;
        }
        Time.timeScale = 0;
        MenuManager.Instance.EndGame("You run out of coal during travelling, the train can go no further. Bugs will be upon you any moment.");
    }
    public void LoseGameRunOutOfCoalStationary()
    {
        if (_isLosingGame)
            return;
        _isLosingGame = true;
        _drive = false;
        StopAllCoroutines();
        Player.Instance.SnapPlayerToTrain();
        MenuManager.Instance.HideTrainMenu();
        Time.timeScale = 0;
        MenuManager.Instance.EndGame("You have no coal to start the engine. Bugs will breach the train soon.");
    }
    public void LoseGameRunOutOfFood()
    {
        if (_isLosingGame)
            return;
        _isLosingGame = true;
        MenuManager.Instance.EndGame("You have ran out of food, but your employees found an alternative source. Train continues without you.");
        _drive = false;
        StopAllCoroutines();
        StartCoroutine(DriveForever());
    }
    private IEnumerator DriveForever()
    {
        _rigidBody.constraints = RigidbodyConstraints2D.None;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        while (true)
        {
            _rigidBody.velocity = Vector2.right * 9.90f;
            Player.Instance.SnapPlayerToTrain();
            yield return null;
        }
    }
    public void LoseGameBugsCameTooClose()
    {
        if (_isLosingGame)
            return;
        _isLosingGame = true;
        StopAllCoroutines();
        MenuManager.Instance.EndGame("Bugs breached the train before you started the engine.");
        Time.timeScale = 0;
        MenuManager.Instance.HideTrainMenu();
    }
    public void LoseGameOutOfEmployees()
    {
        if (_isLosingGame)
            return;
        _isLosingGame = true;
        StopAllCoroutines();
        MenuManager.Instance.EndGame("You are all alone. The train can't go anywhere without workforce.");
        MenuManager.Instance.HideTrainMenu();
        Time.timeScale = 0;
    }
    public void LoseGameBugAteYou()
    {
        if (_isLosingGame)
            return;
        _isLosingGame = true;
        StopAllCoroutines();
        Time.timeScale = 0;
        MenuManager.Instance.EndGame("You were not careful enough, don't stray too far from the train.");
        MenuManager.Instance.HideTrainMenu();
    }
}
