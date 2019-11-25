﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1. Movement based of camera
/// 2. stop and face dircetion when input is absent
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private Transform cam;


    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _pushSpeed = 2f;

    

    [SerializeField] private float _turnSpeed = 10f;
    [SerializeField] private float _gravity = 20f;
    [SerializeField] private float _jumpSpeed = 10f;

    [SerializeField] private bool _jump;


    private Vector2 _input;
    private float _angle;

    private Quaternion _targetRotation;
    private CharacterController _controller;

    private Vector3 _movementDirection;

    public bool carryingObject;

    private float _originalRange;

    public Vector3 MovementDirection
    {
        get => _movementDirection;
    }
    
    public Vector3 ControllerVeclocity
    {
        get => _controller.velocity;
        set => _controller.Move(value);
    }

    public bool grounded;

    [SerializeField] private bool _pushing;

    public bool Pushing
    {
        get => _pushing;
        set => _pushing = value;
    }

    private float _timeDelta;

    private InputSetUp _inputSetUp;
    
    
    

    
    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        _controller = GetComponent<CharacterController>();
        _inputSetUp = GetComponent<InputSetUp>();

        _originalRange = _controller.radius;
    }

    private void Update()
    {
        if (carryingObject)
        {
            _controller.radius = _originalRange * 2f;
        }
        else
        {
            _controller.radius = _originalRange;
        }

        _input = _inputSetUp.LeftStick;

        grounded = _controller.isGrounded;
        _timeDelta = Time.deltaTime;

        CalculateDirection();

        if (_controller.isGrounded)
        {
            if (Mathf.Abs(_input.magnitude) > 0.05f && !_pushing)
            {
                Rotate();
            }

            SetMove();

            if (_inputSetUp.Controls.PlayerFreeMovement.Jump.triggered && _jump && ! _pushing)
            {
                Jump();
            }
        }


        ApplyGravity();

        _controller.Move(_movementDirection * _timeDelta);
    }

    /// <summary>
    /// Calculates the direction of movement
    /// </summary>
    void CalculateDirection()
    {
        _angle = Mathf.Atan2(_input.x, _input.y);
        _angle = Mathf.Rad2Deg * _angle;
        _angle += cam.eulerAngles.y;
    }

    /// <summary>
    /// Rotate towards the target direction
    /// </summary>
    void Rotate()
    {
        _targetRotation = Quaternion.Euler(0, _angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _turnSpeed * _timeDelta);
    }

    /// <summary>
    /// Sets the moveDirections along forward axis by the speed variable
    /// </summary>
    void SetMove()
    {
        Vector3 right = cam.right;
        Vector3 forward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;

        Vector3 movement = (right * _input.x) + (forward * _input.y);

        if (!_pushing)
        {
            _movementDirection = movement;
            _movementDirection *= _speed;
        }
        else
        {
            if (Vector3.Angle(transform.forward, movement) > 120f)
            {
                _movementDirection = -transform.forward;
                _movementDirection *= _pushSpeed;
            }
            else if (Vector3.Angle(transform.forward, movement) < 60f)
            {
                _movementDirection = transform.forward;
                _movementDirection *= _pushSpeed * _input.magnitude;
            }
        }
    }

    /// <summary>
    /// Apply gravity to the moveDirections y axis
    /// </summary>
    void ApplyGravity()
    {
        _movementDirection.y -= _gravity * _timeDelta;
    }


    /// <summary>
    /// Set the move directions y to the jump speed
    /// this has to be called after set move, otherwise the y gets reset
    /// </summary>
    void Jump()
    {
        _movementDirection.y = _jumpSpeed;
    }
}