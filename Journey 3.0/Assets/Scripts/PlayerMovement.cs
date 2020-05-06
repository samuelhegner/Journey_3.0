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

    [SerializeField] private Animator _anim;

    public Animator Anim
    {
        set => _anim = value;
        get => _anim;
    }


    [SerializeField] private float _speed = 5f;

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    [SerializeField] private float _pushSpeed = 2f;

    

    [SerializeField] private float _turnSpeed = 10f;
    [SerializeField] private float _gravity = 20f;
    [SerializeField] private float _jumpSpeed = 10f;

    [SerializeField] private bool _jump;

    [SerializeField] private float _minWalkSpeed = 1f;
    [SerializeField] private float _maxWalkSpeed = 20f;

    public float MaxWalkSpeed
    {
        get => _maxWalkSpeed;
        set => _maxWalkSpeed = value;
    }


    private Vector2 _input;
    private float _angle;

    private Quaternion _targetRotation;
    private CharacterController _controller;

    private Vector3 _movementDirection;

    public bool carryingObject;

    private float _originalRange;

    public bool pushingForward;

    public PushCollisionDetection _pushCollisionDetection;

    public bool frozen;

    [SerializeField] private GroundRayCast _groundRayCast;

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


    public RayInfo info;

    public LayerMask mask;

    [SerializeField] private bool pushingBoulder;
    
    
    [SerializeField] private AudioSource _walkSource;


    private bool _remoteControl;
    private GameObject _objectToFollow;
    
    
    bool ccSwitch;

    private Vector3 lastPos;

    [SerializeField] private float remoteAnimationVelocityMultiplier;


    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        _controller = GetComponent<CharacterController>();
        _inputSetUp = GetComponent<InputSetUp>();

        _originalRange = _controller.radius;

        FadeToBlack.instance.SetBlack(false);
        print("called");
    }

    private void Update()
    {
        if (_controller == null)
        {
            Start();
        }

        if (carryingObject)
        {
            if (ccSwitch == true)
            {
                _controller.radius = _originalRange * 1.15f;
                ccSwitch = false;
            }

        }
        else
        {
            if (ccSwitch == false)
            {
                print("Switched back");
                _controller.radius = _originalRange;
                ccSwitch = true;
            }
        }

        if (!_remoteControl)
        {
            if (frozen)
            {
                _input = Vector2.zero;
            }
            else
            {
                _input = _inputSetUp.LeftStick;
            }
            
            
            
            grounded = _controller.isGrounded;
            _timeDelta = Time.deltaTime;

            if (!frozen)
            {
                CalculateDirection();
                
                if (Mathf.Abs(_input.magnitude) > 0.05f && !_pushing)
                {
                    Rotate();
                }

                if (_controller.isGrounded)
                {
                    SetMove();

                    if (_inputSetUp.Controls.PlayerFreeMovement.Jump.triggered && _jump && ! _pushing)
                    {
                        Jump();
                    }
                }
                
                ApplyGravity();

                _controller.Move(_movementDirection * _timeDelta);
                if (_controller.velocity.magnitude >= 0.5f)
                {
                    float walkSoundPitch = Map(_controller.velocity.magnitude, 0f, _speed, 0.6f, 1.35f);

                    _walkSource.pitch = walkSoundPitch;
            
                    if (!_walkSource.isPlaying)
                    {
                        _walkSource.Play();
                    }
                }else
                {
                    _walkSource.Stop();
                }

                SetAnimation();
            }
        }
        else
        {
            lastPos = transform.position;
            
            transform.position = _objectToFollow.transform.position;
            
            transform.rotation = _objectToFollow.transform.rotation;
            
            SetRemoteAnimation();
        }
    }

    private void SetRemoteAnimation()
    {

        float velocity = Vector3.Distance(lastPos, transform.position) * remoteAnimationVelocityMultiplier;
        
        _anim.SetFloat("velocity", velocity);

        float walkSpeed = Map(velocity, 0f, _speed, _minWalkSpeed, _maxWalkSpeed);
        
        _anim.SetFloat("walkSpeed", walkSpeed);

        if (velocity > 0.1f)
        {
            float walkSoundPitch = Map(velocity, 0f, _speed, 0.6f, 1.35f);

            _walkSource.pitch = walkSoundPitch;
            
            if (!_walkSource.isPlaying)
            {
                _walkSource.Play();
            }
        }else
        {
            _walkSource.Stop();
        }

        
    }

    public void StartRemoteControlledMovement(GameObject objToFollow)
    {
        _objectToFollow = objToFollow;
        _remoteControl = true;
    }

    public void StopRemoteControlledMovement()
    {
        _remoteControl = false;
    }

    private void SetAnimation()
    {
        _anim.SetFloat("velocity", _controller.velocity.magnitude);

        float walkSpeed = Map(_controller.velocity.magnitude, 0f, _speed, _minWalkSpeed, _maxWalkSpeed);
        
        _anim.SetFloat("walkSpeed", walkSpeed);
        
        _anim.SetBool("pushing", _pushing);
    }

    public void PickUpLow()
    {
        _anim.SetTrigger("pickUp");
    }

    public void PickUpHigh()
    {
        _anim.SetTrigger("grab");
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
        float angleToStraight = Quaternion.Angle(_targetRotation, transform.rotation);

        float adjustedSpeed = Map(angleToStraight, 180, 0, 0f, _speed);
        
        Vector3 right = cam.right;
        Vector3 forward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;

        Vector3 movement = (right * _input.x) + (forward * _input.y);

        movement = Quaternion.AngleAxis(_groundRayCast.SlopeAngle, transform.right) * movement;
        

        Debug.DrawRay(transform.position + transform.up, movement * 10f);
        
        
        if (!_pushing)
        {
            gameObject.layer = 11;
            _movementDirection = movement;
            _movementDirection *= adjustedSpeed;
            pushingBoulder = false;
        }
        else
        {
            gameObject.layer = 22;
            float localPushDirection;
            
            if (Vector3.Angle(transform.forward, movement) > 120f && _input.magnitude > 0.1f)
            {
                _movementDirection = -transform.forward;
                _movementDirection *= _pushSpeed * _input.magnitude;
                localPushDirection = -1f * _input.magnitude;
                pushingBoulder = true;
                pushingForward = false;
            }
            else if (Vector3.Angle(transform.forward, movement) < 60f && _input.magnitude > 0.1f)
            {
                Debug.DrawRay(info.position, transform.forward * info.distance);
                
                Ray ray = new Ray(info.position, transform.forward);

                print(_pushCollisionDetection.IsCollidingWithWall());


                if (!Physics.SphereCast(ray, 0.2f, out RaycastHit hit, info.distance, mask) && !_pushCollisionDetection.IsCollidingWithWall())
                {
                    _movementDirection = transform.forward;
                    _movementDirection *= _pushSpeed * _input.magnitude;
                    localPushDirection = 1f * _input.magnitude;
                    pushingBoulder = true;
                    pushingForward = true;
                }
                else
                {
                    if (hit.transform.gameObject != null)
                    {
                        print(hit.transform.gameObject != null);
                    }

                    if (_pushCollisionDetection.IsCollidingWithWall())
                    {
                        print(_pushCollisionDetection.CollidingObjects);
                    }

                    pushingBoulder = false;
                    _movementDirection = forward * 0f;
                    localPushDirection = 0;
                }
            }
            else
            {
                _movementDirection = forward * 0f;
                pushingBoulder = false;
                localPushDirection = 0f;
            }
            _anim.SetFloat("pushDirection", localPushDirection);
        }
    }


    public bool PushingBoulder()
    {
        return pushingBoulder;
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
    
    public float Map(float a, float b, float c, float d, float e)
    {
        
        float cb = c - b;
        float de = e - d;
        float howFar = (a - b) / cb;
        return d + howFar * de;
        
        //float a = value you want mapped t
    }


    public float ReturnCurrentClipLength()
    {
        return _anim.GetCurrentAnimatorClipInfo(0).Length;
    }


    public void DisableThis()
    {
        _walkSource.Stop();
        _anim.SetFloat("velocity", 0f);
        frozen = true;
    }
    
    public void EnableThis()
    {
        frozen = false;
    }
}