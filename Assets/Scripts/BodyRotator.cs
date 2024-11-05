using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BodyRotator : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (_rigidbody.velocity == Vector2.zero)
            return;
        
        float targetAngle = Vector2.SignedAngle(Vector2.up, _rigidbody.velocity.normalized);
        float smoothedAngle = Mathf.LerpAngle(_rigidbody.rotation, targetAngle, 10 * Time.deltaTime);
        _rigidbody.rotation = smoothedAngle;
    }
}
