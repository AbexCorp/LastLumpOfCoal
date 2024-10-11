using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Camera _camera;


    private float _length;
    private float _startPositionX;

    [SerializeField]
    private float _paralaxValue;



    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _camera = Camera.main;
        _length = _spriteRenderer.bounds.size.x;
        _startPositionX = transform.position.x;
    }
    void Update()
    {
        float relativeMovement = (_camera.transform.position.x * (1 - _paralaxValue));
        float distance = _camera.transform.position.x * _paralaxValue;

        transform.position = new Vector2(_startPositionX + distance, transform.position.y);
        float cameraWidth = _camera.orthographicSize * _camera.aspect * 2f;

        if (relativeMovement > _startPositionX + _length - cameraWidth / 2)
            _startPositionX += _length;
        else if (relativeMovement < _startPositionX - _length + cameraWidth / 2)
            _startPositionX -= _length;
    }
}
