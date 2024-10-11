using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BugScript : MonoBehaviour
{
    public Transform CenterPoint => Train.Instance.transform; 
    public float RadiusOutside => Danger.Instance.CurrentRadius + 5;
    public float RadiusInside => Danger.Instance.CurrentRadius;

    [SerializeField]
    private float _bugSpeed;

    private Vector2 _randomDirection;
    private bool _isOutsideDonut = true;

    private void Start()
    {
        SetRandomDirection();
        StartCoroutine(ChangeDirectionPeriodically());
    }
    private void Update()
    {
        MoveBug();
        if (IsOutsideDonut())
        {
            if (!_isOutsideDonut)
                _isOutsideDonut = true;
            SetRandomDirection();
            StartCoroutine(CheckIfStuck());
        }
        else
            _isOutsideDonut = false;
    }

    private void MoveBug()
    {
        transform.Translate(_randomDirection * _bugSpeed * Time.deltaTime);
    }
    private void SetRandomDirection()
    {
        _randomDirection = Random.insideUnitCircle.normalized;
    }
    private bool IsOutsideDonut()
    {
        float distanceFromCenter = Vector2.Distance(transform.position, CenterPoint.position);
        return distanceFromCenter > RadiusOutside || distanceFromCenter < RadiusInside;
    }
    private IEnumerator ChangeDirectionPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.2f,1.5f));
            SetRandomDirection();
        }
    }
    private IEnumerator CheckIfStuck()
    {
        yield return new WaitForSeconds(0.5f);
        if (_isOutsideDonut)
        {
            TeleportToRandomPositionInDonut();
            _isOutsideDonut = false;
        }
    }
    void TeleportToRandomPositionInDonut()
    {
        Vector2 newPosition;
        do
        {
            newPosition = Random.insideUnitCircle * RadiusOutside;
        }
        while (newPosition.magnitude < RadiusInside);

        transform.position = (Vector2)CenterPoint.position + newPosition;
    }
}
