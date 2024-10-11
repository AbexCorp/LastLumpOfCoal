using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Danger : MonoBehaviour
{
    public static Danger Instance;

    #region Bugs

    [SerializeField]
    private DangerLevel _dangerLevel = DangerLevel.Safe;
    public DangerLevel DangerLevel => _dangerLevel;

    private float _currentRadius;
    public float CurrentRadius => _currentRadius;


    [SerializeField] private float _safeRadius = 1;
    [SerializeField] private float _riskyRadius = 1;
    [SerializeField] private float _dangerousRadius = 1;
    [SerializeField] private float _deadlyRadius = 1;

    #endregion


    #region Resources

    [SerializeField]
    private NaturalResourceScarcity _coalScarcity = NaturalResourceScarcity.Few;
    [SerializeField]
    private NaturalResourceScarcity _foodScarcity = NaturalResourceScarcity.Few;
    public NaturalResourceScarcity CoalScarcity => _coalScarcity;
    public NaturalResourceScarcity FoodScarcity => _foodScarcity;


    [SerializeField] private int _fewProgressRequired = 10;
    [SerializeField] private int _limitedProgressRequired = 10;
    [SerializeField] private int _sparseProgressRequired = 10;
    [SerializeField] private int _minisculeProgressRequired = 10;
    [SerializeField] private int _depletedProgressRequired = 10;

    #endregion


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _currentRadius = DangerLevelToRadius(_dangerLevel);
        Train.Instance.OnDoorOpen += StartTimer;
        Train.Instance.OnDoorClose += StopTimer;
        MenuManager.Instance.UpdateResourceLimit();
    }






    public void PrepNewMap()
    {
        int rng = (int)Mathf.Round(UnityEngine.Random.Range(minInclusive: 1, maxInclusive: 100));
        if (rng > 0 && rng <= 13)
            _dangerLevel = DangerLevel.Safe;
        else if(rng > 13 && rng <= 38)
            _dangerLevel = DangerLevel.Risky;
        else if(rng > 38 && rng <= 83)
            _dangerLevel = DangerLevel.Dangerous;
        else if(rng > 83 && rng <= 100)
            _dangerLevel = DangerLevel.Deadly;

        rng = (int)Mathf.Round(UnityEngine.Random.Range(minInclusive: 1, maxInclusive: 100));
        if (rng > 0 && rng <= 10)
            _coalScarcity = NaturalResourceScarcity.Few;
        else if(rng > 10 && rng <= 30)
            _coalScarcity = NaturalResourceScarcity.Limited;
        else if(rng > 30 && rng <= 65)
            _coalScarcity = NaturalResourceScarcity.Sparse;
        else if(rng > 65 && rng <= 87)
            _coalScarcity = NaturalResourceScarcity.Miniscule;
        else if(rng > 87 && rng <= 100)
            _coalScarcity = NaturalResourceScarcity.Depleted;

        rng = (int)Mathf.Round(UnityEngine.Random.Range(minInclusive: 1, maxInclusive: 100));
        if (rng > 0 && rng <= 10)
            _foodScarcity = NaturalResourceScarcity.Few;
        else if(rng > 10 && rng <= 30)
            _foodScarcity = NaturalResourceScarcity.Limited;
        else if(rng > 30 && rng <= 65)
            _foodScarcity = NaturalResourceScarcity.Sparse;
        else if(rng > 65 && rng <= 87)
            _foodScarcity = NaturalResourceScarcity.Miniscule;
        else if(rng > 87 && rng <= 100)
            _foodScarcity = NaturalResourceScarcity.Depleted;


        _currentRadius = DangerLevelToRadius(DangerLevel);
        MenuManager.Instance.UpdateResourceLimit();
        BugManager.Instance.SpawnBugs();
    }
    private void StartTimer()
    {
        stopTimer = false;
        _currentRadius = DangerLevelToRadius(DangerLevel);
        StartCoroutine(DangerTimer());
    }
    bool stopTimer = false;
    private void StopTimer()
    {
        stopTimer = true;
    }
    private IEnumerator DangerTimer()
    {
        yield return new WaitForSeconds(0.25f);

        int rng = (int)Mathf.Round(UnityEngine.Random.Range(minInclusive: 1, maxInclusive: 100));
        if(_currentRadius <= _deadlyRadius)
        {
            if (rng > 69)
                Resources.Instance.KillWorkingEmployee();
        }
        else if(_currentRadius <= _dangerousRadius)
        {
            if (rng > 84)
                Resources.Instance.KillWorkingEmployee();
        }
        else if(_currentRadius <= _riskyRadius)
        {
            if (rng > 91)
                Resources.Instance.KillWorkingEmployee();
        }

        AdvanceDanger();

        if (!stopTimer)
            StartCoroutine(DangerTimer());
        else
            stopTimer = false;
    }
    private void AdvanceDanger()
    {
        _currentRadius -= 0.05f;
        if (_currentRadius <= 0)
            Train.Instance.LoseGameBugsCameTooClose();
    }






    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, DangerLevelToRadius(_dangerLevel));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _currentRadius);
    }



    public float DangerLevelToRadius(DangerLevel danger)
    {
        switch(danger)
        {
            case DangerLevel.Safe:
                return _safeRadius;
            case DangerLevel.Risky:
                return _riskyRadius;
            case DangerLevel.Dangerous:
                return _dangerousRadius;
            case DangerLevel.Deadly:
                return _deadlyRadius;
            default:
                return 0.1f;
        }
    }
    public int NaturalResourceScarcityToProgressNeeded(NaturalResourceScarcity scarcity)
    {
        switch(scarcity)
        {
            case NaturalResourceScarcity.Few:
                return _fewProgressRequired;
            case NaturalResourceScarcity.Limited:
                return _limitedProgressRequired;
            case NaturalResourceScarcity.Sparse:
                return _sparseProgressRequired;
            case NaturalResourceScarcity.Miniscule:
                return _minisculeProgressRequired;
            case NaturalResourceScarcity.Depleted:
                return _depletedProgressRequired;
            default:
                return 1000000;
        }
    }

}
public enum DangerLevel
{
    Safe = 0,
    Risky = 1,
    Dangerous = 2,
    Deadly = 3
}
public enum NaturalResourceScarcity
{
    Few = 0,
    Limited = 1,
    Sparse = 2,
    Miniscule = 3,
    Depleted = 4
}
