using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BugManager : MonoBehaviour
{
    public static BugManager Instance;
    [SerializeField]
    private GameObject _bugPrefab;
    private List<GameObject> _bugs;

    private void Awake()
    {
        Instance = this;
        _bugs = new List<GameObject>();
    }


    public void SpawnBugs()
    {
        if(_bugs.Count == 0)
        {
            for(int i = 0; i < 2000; i++)
            {
                Spawn();
            }
        }
        else
        {
            foreach (GameObject bug in _bugs)
            {;
                bug.SetActive(true);
            }
        }
    }
    private System.Random random = new();
    private Vector2[] spawnPoints = { Vector2.up, Vector2.down,
        new Vector2(0.707f,0.707f), new Vector2(-0.707f,-0.707f), new Vector2(-0.707f,0.707f), new Vector2(0.707f,-0.707f), };
    private void Spawn()
    {
        int rng = Random.Range(minInclusive: 1, maxExclusive: 5);
        Vector2 spawn = spawnPoints[random.Next(0, spawnPoints.Length)];
        spawn = spawn * (Danger.Instance.DangerLevelToRadius(Danger.Instance.DangerLevel) + 2);
        spawn = spawn + Train.Instance.TrainPosition;
        GameObject bug = Instantiate(_bugPrefab, spawn, Quaternion.identity);
        _bugs.Add(bug);
    }
    public void CullBugs()
    {
        foreach (GameObject bug in _bugs)
        {
            bug.SetActive(false);
        }
        _bugs.Clear();
    }
}
