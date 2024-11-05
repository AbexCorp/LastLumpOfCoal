using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public static Resources Instance;

    [SerializeField]
    private Vector2 _coalLocation;
    [SerializeField]
    private Vector2 _foodLocation;

    public Vector2 CoalLocation => _coalLocation;
    public Vector2 FoodLocation => _foodLocation;


    public int NumberOfEmployees { get; private set; }
    public int EmployeesOnCoal { get; private set; }
    public int EmployeesOnFood { get; private set; }

    public int Coal { get; private set; }
    public int Food { get; private set; }


    private int _coalProgress = 0;
    private int _foodProgress = 0;



    private void Awake()
    {
        Instance = this;
        NumberOfEmployees = 13;
        EmployeesOnCoal = 0;
        EmployeesOnFood = 0;
        Coal = 9;
        Food = 20;
    }
    private void Start()
    {
        Train.Instance.OnDoorOpen += StartTimer;
        Train.Instance.OnDoorClose += StopTimer;
        MenuManager.Instance.UpdateResourceBar();
        Train.Instance.OnDoorClose += CullVisualWorkers;
        Train.Instance.OnDoorOpen += SpawnVisualWorkers;
    }



    private void StartTimer()
    {
        stopTimer = false;
        StartCoroutine(MiningTimer());
    }
    bool stopTimer = false;
    private void StopTimer()
    {
        stopTimer = true;
    }
    private IEnumerator MiningTimer()
    {
        yield return new WaitForSeconds(1);

        MineCoal();
        GatherFood();

        if (!stopTimer)
            StartCoroutine(MiningTimer());
        else
            stopTimer = false;
    }
    


    public void ChangeCoalEmployees(int newEmployeeNumber)
    {
        EmployeesOnCoal = newEmployeeNumber;
    }
    public void ChangeFoodEmployees(int newEmployeeNumber)
    {
        EmployeesOnFood = newEmployeeNumber;
    }


    private void MineCoal()
    {
        _coalProgress += EmployeesOnCoal;
        if(_coalProgress >= Danger.Instance.NaturalResourceScarcityToProgressNeeded(Danger.Instance.CoalScarcity))
        {
            Coal++;
            _coalProgress = _coalProgress % Danger.Instance.NaturalResourceScarcityToProgressNeeded(Danger.Instance.CoalScarcity);
        }
        MenuManager.Instance.UpdateResourceBar();
    }
    private void GatherFood()
    {
        _foodProgress += EmployeesOnFood * 2;
        if(_foodProgress >= Danger.Instance.NaturalResourceScarcityToProgressNeeded(Danger.Instance.FoodScarcity))
        {
            Food++;
            _foodProgress = _foodProgress % Danger.Instance.NaturalResourceScarcityToProgressNeeded(Danger.Instance.FoodScarcity);
        }
        MenuManager.Instance.UpdateResourceBar();
    }
    public void DepleteCoal(int amount)
    {
        Coal -= amount;
        if(Coal <= 0)
        {
            Train.Instance.LoseGameRunOutOfCoalDriving();
        }
        MenuManager.Instance.UpdateResourceBar();
    }
    public void DepleteFood(int amount)
    {
        Food -= amount;
        if (Food * -1 >= NumberOfEmployees)
        {
            MenuManager.Instance.UpdateResourceBar();
            Train.Instance.LoseGameRunOutOfFood();
            return;
        }

        if(Food < 0)
        {
            KillEmployees(Food * -1);
            Food = 0;
        }

        MenuManager.Instance.UpdateResourceBar();
    }
    public void KillEmployees(int amount)
    {
        NumberOfEmployees -= amount;
        if(NumberOfEmployees <= 0)
        {
            Train.Instance.LoseGameOutOfEmployees();
        }
        MenuManager.Instance.UpdateResourceBar();
        MenuManager.Instance.ChangeSliderAfterDeath();
    }
    public void KillWorkingEmployee()
    {
        if (EmployeesOnCoal + EmployeesOnFood == 0)
            return;
        int rng = (int)Mathf.Round(UnityEngine.Random.Range(minInclusive: 1, maxInclusive: EmployeesOnCoal + EmployeesOnFood));
        Worker.WorkerType? type = null;
        if(rng <= EmployeesOnCoal && EmployeesOnCoal > 0)
        {
            NumberOfEmployees -= 1;
            EmployeesOnCoal -= 1;
            type = Worker.WorkerType.Coal;
        }
        else if(rng > EmployeesOnCoal && rng <= EmployeesOnFood && EmployeesOnFood > 0)
        {
            NumberOfEmployees -= 1;
            EmployeesOnFood -= 1;
            type = Worker.WorkerType.Food;
        }

        if(type != null)
            _workers.FirstOrDefault(x => x != null && x.ResourceWorkerType == type).KillWorker();

        if(NumberOfEmployees <= 0)
        {
            Train.Instance.LoseGameOutOfEmployees();
        }
        MenuManager.Instance.UpdateResourceBar();
        MenuManager.Instance.ChangeSliderAfterDeath();
    }
    public void CullRemainingEmployees()
    {
        NumberOfEmployees -= EmployeesOnCoal;
        NumberOfEmployees -= EmployeesOnFood;
        EmployeesOnCoal = 0;
        EmployeesOnFood = 0;
    }



    [SerializeField]
    private Worker _workerPrefab;
    private List<Worker> _workers = new();
    private void SpawnVisualWorkers()
    {
        for(int i = 0; i < EmployeesOnCoal; i++)
        {
            Worker worker = Instantiate(_workerPrefab, Train.Instance.TrainPosition, Quaternion.identity);
            worker.ChooseWorkerType(Worker.WorkerType.Coal);
            _workers.Add(worker);
        }
        for(int i = 0; i < EmployeesOnFood; i++)
        {
            Worker worker = Instantiate(_workerPrefab, Train.Instance.TrainPosition, Quaternion.identity);
            worker.ChooseWorkerType(Worker.WorkerType.Food);
            _workers.Add(worker);
        }
    }
    public void CallBackVisualWorkers()
    {
        foreach(var x in _workers)
            x.ReturnToTrain();
    }
    private void CullVisualWorkers()
    {
        foreach (var x in _workers)
        {
            if (x == null)
                continue;
            x.KillWorker();
        }
        _workers.Clear();
    }
    public void VisualWorkerReturned(Worker.WorkerType type)
    {
        switch(type)
        {
            case Worker.WorkerType.Coal:
                EmployeesOnCoal--;
                break;
            case Worker.WorkerType.Food:
                EmployeesOnFood--;
                break;
        }
        MenuManager.Instance.UpdateResourceBar();
        MenuManager.Instance.ChangeSliderAfterDeath();
    }





    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_coalLocation + (Vector2)transform.position, Vector3.one * 0.5f);
        Gizmos.DrawWireCube(_foodLocation + (Vector2)transform.position, Vector3.one * 0.5f);
    }
}
