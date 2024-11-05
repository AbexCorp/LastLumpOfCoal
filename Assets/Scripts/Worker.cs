using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidbody;
    public WorkerType ResourceWorkerType {  get; private set; }

    private void Awake()
    {
        if(_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if (isWalkingToResources)
        {
            if (Vector2.Distance(_rigidbody.position, _walkPoint) < 0.1f)
            {
                _rigidbody.velocity = Vector2.zero;
                isGathering = true;
                isWalkingToResources = false;
                return;
            }
            WalkToResources();
        }
        else if (isGathering)
            Gather();
        else if (isReturningToTrain)
        {
            WalkToTrain();
        }
    }


    private bool isWalkingToResources = true;
    private bool isGathering = false;
    private bool isReturningToTrain = false;

    private bool isInsideTheTrain = true;


    private Vector2 _resourcePoint;
    private Vector2 _walkPoint;
    private void WalkToResources()
    {
        _rigidbody.velocity = (_walkPoint - _rigidbody.position).normalized * 1.5f;
    }

    /*
    float targetAngle = Vector2.SignedAngle(Vector2.up, _rigidbody.velocity.normalized);
        float smoothedAngle = Mathf.LerpAngle(_rigidbody.rotation, targetAngle, 10 * Time.deltaTime);
        _rigidbody.rotation = smoothedAngle;
    */
    private void Gather()
    {
        _rigidbody.velocity = Vector2.zero;
        Vector2 direction = (_resourcePoint - _rigidbody.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        _rigidbody.rotation = angle;
    }
    public void ReturnToTrain()
    {
        isWalkingToResources = false;
        isGathering = false;
        isReturningToTrain = true;
        _walkPoint = Train.Instance.TrainPosition;
        _walkPoint.x = _walkPoint.x + Random.Range(-0.2f, 0.2f);
    }
    private void WalkToTrain()
    {
        _rigidbody.velocity = (_walkPoint - _rigidbody.position).normalized * 1.5f;
    }


    public void EnterTrain()
    {
        isInsideTheTrain = true;
        if (!isReturningToTrain)
            return;
        KillWorker();
    }
    public void ExitTrain() => isInsideTheTrain = false;

    public void KillWorker()
    {
        if(isInsideTheTrain)
        {
            //Return safely, no break
        }
        Destroy(gameObject);
    }


    public void ChooseWorkerType(WorkerType workerType)
    {
        ResourceWorkerType = workerType;
        _resourcePoint = ResourceWorkerType == WorkerType.Coal ? Resources.Instance.CoalLocation : Resources.Instance.FoodLocation;
        _resourcePoint = _resourcePoint + Train.Instance.TrainPosition;
        _rigidbody.position = new Vector2(Train.Instance.TrainPosition.x + Random.Range(-0.2f, 0.2f), Train.Instance.TrainPosition.y);
        Physics2D.SyncTransforms();
        _walkPoint = _resourcePoint + Random.insideUnitCircle;
    }
    public enum WorkerType
    {
        Coal = 0,
        Food = 1
    }
}
