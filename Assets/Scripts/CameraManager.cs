using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    private Vector2 _delta;
    private bool _isRotating;
    public bool isBusy;

    private float _xRotation;

    [SerializeField] private float rotationSpeed = .5f;

    private void Awake()
    {
        _xRotation = transform.rotation.eulerAngles.x;
        isBusy = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _delta = context.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        _isRotating = context.started || context.performed;
        Debug.Log(_isRotating);
        if (context.canceled)
        {
            isBusy = true;
            SnapRotation();
        }
    }

    private void LateUpdate()
    {
        if (_isRotating)
        {
            // Debug.Log(_isRotating);
            transform.Rotate(new Vector3(_xRotation, _delta.x * rotationSpeed, 0));
            transform.rotation = Quaternion.Euler(_xRotation, transform.rotation.eulerAngles.y, 0.0f);
        }
    }
    
    private void SnapRotation()
    {
        transform.DORotate(SnappedVector(), 0.5f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() => { isBusy = false; });
    }
    
    private Vector3 SnappedVector()
    {
        var endValue = 0.0f;
        var currentY = Mathf.Ceil(transform.rotation.eulerAngles.y);


        endValue = currentY switch
        {
            >= 0 and <= 90 => 45.0f,
            >= 91 and <= 180 => 135.0f,
            >= 181 and <= 270 => 225.0f,
            _ => 315.0f
        };

        return new Vector3(_xRotation, endValue, 0f);
    }
}
