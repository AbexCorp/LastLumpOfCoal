using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField]
    private GameObject _trainMenu;
    [SerializeField]
    private GameObject _openDoorButton;
    [SerializeField]
    private GameObject _closeDoorButton;

    [Space]
    [SerializeField]
    private TMP_Text _mapSafety;
    [SerializeField]
    private Slider _coalWorkersSlider;
    [SerializeField]
    private Slider _foodWorkersSlider;

    [Space]
    [SerializeField]
    private TMP_Text _coalWorkersText;
    [SerializeField]
    private TMP_Text _foodWorkersText;

    [SerializeField]
    private TMP_Text _coalLimit;
    [SerializeField]
    private TMP_Text _foodLimit;


    [Space]
    [Space]
    [SerializeField]
    private TMP_Text _employeeAmount;
    [SerializeField]
    private TMP_Text _coalAmount;
    [SerializeField]
    private TMP_Text _foodAmount;
    [SerializeField]
    private TMP_Text _distanceTravelled;



    [Space]
    [SerializeField]
    private GameObject _endGameScreen;
    [SerializeField]
    private TMP_Text _score;
    [SerializeField]
    private TMP_Text _reason;





    private void Awake()
    {
        Instance = this;
        _coalWorkersSlider.wholeNumbers = true;
        _foodWorkersSlider.wholeNumbers = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        _coalWorkersSlider.maxValue = Resources.Instance.NumberOfEmployees - Resources.Instance.EmployeesOnFood;
        _foodWorkersSlider.maxValue = Resources.Instance.NumberOfEmployees - Resources.Instance.EmployeesOnCoal;
        _coalWorkersText.text = Resources.Instance.EmployeesOnCoal.ToString();
        _foodWorkersText.text = Resources.Instance.EmployeesOnFood.ToString();

        ShowTrainMenu(doorsOpen: false);
    }



    public void UpdateResourceLimit()
    {
        _coalLimit.text = Danger.Instance.CoalScarcity.ToString();
        _foodLimit.text = Danger.Instance.FoodScarcity.ToString();
        _mapSafety.text = Danger.Instance.DangerLevel.ToString();
    }
    public void SlidersChange()
    {
        Resources.Instance.ChangeCoalEmployees((int)_coalWorkersSlider.value);
        Resources.Instance.ChangeFoodEmployees((int)_foodWorkersSlider.value);

        _coalWorkersSlider.maxValue = Resources.Instance.NumberOfEmployees - Resources.Instance.EmployeesOnFood;
        _foodWorkersSlider.maxValue = Resources.Instance.NumberOfEmployees - Resources.Instance.EmployeesOnCoal;

        _coalWorkersText.text = Resources.Instance.EmployeesOnCoal.ToString();
        _foodWorkersText.text = Resources.Instance.EmployeesOnFood.ToString();
    }
    public void ChangeSliderAfterDeath()
    {
        _coalWorkersSlider.value = Resources.Instance.EmployeesOnCoal;
        _foodWorkersSlider.value = Resources.Instance.EmployeesOnFood;

        _coalWorkersSlider.maxValue = Resources.Instance.NumberOfEmployees - Resources.Instance.EmployeesOnFood;
        _foodWorkersSlider.maxValue = Resources.Instance.NumberOfEmployees - Resources.Instance.EmployeesOnCoal;

        _coalWorkersText.text = Resources.Instance.EmployeesOnCoal.ToString();
        _foodWorkersText.text = Resources.Instance.EmployeesOnFood.ToString();
    }
    public void LockSliders()
    {
        _coalWorkersSlider.interactable = false;
        _foodWorkersSlider.interactable= false;
    }
    public void UnlockSliders()
    {
        _coalWorkersSlider.interactable = true;
        _foodWorkersSlider.interactable= true;
    }
    public void ResetEmployees()
    {
        _coalWorkersSlider.value = 0;
        _foodWorkersSlider.value = 0;
        SlidersChange();
        Resources.Instance.CallBackVisualWorkers();
    }
    public void CloseDoors()
    {
        HideTrainMenu();
        Train.Instance.CloseDoor();
    }
    public void OpenDoors()
    {
        Train.Instance.OpenDoor();
        ShowTrainMenu(doorsOpen: true);
    }
    public void ShowTrainMenu(bool doorsOpen)
    {
        _trainMenu.SetActive(true);
        if(doorsOpen)
        {
            _closeDoorButton.SetActive(true);
            _openDoorButton.SetActive(false);
        }
        else
        {
            _closeDoorButton.SetActive(false);
            _openDoorButton.SetActive(true);
        }
    }
    public void HideTrainMenu()
    {
        _trainMenu.SetActive(false);
    }
    public void UpdateResourceBar()
    {
        _employeeAmount.text = "Workers:" + Resources.Instance.NumberOfEmployees.ToString();
        _coalAmount.text = "Coal:" + Resources.Instance.Coal.ToString();
        _foodAmount.text = "Food:" + Resources.Instance.Food.ToString();
    }
    public void UpdateDistanceTravelled()
    {
        _distanceTravelled.text = Train.Instance.DistanceTravelled.ToString() + 'm';
    }
    public void EndGame(string text)
    {
        _endGameScreen.SetActive(true);
        _score.text = $"Travelled: {Train.Instance.DistanceTravelled}";
        _reason.text = text;
    }
}
