﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorMovement : MonoBehaviour
{
    [SerializeField] private float _elevatorMovementSpeed;

    public float ElevatorMovementSpeed
    {
        get => _elevatorMovementSpeed;
        set => _elevatorMovementSpeed = value;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-Vector3.up * _elevatorMovementSpeed * Time.deltaTime);
    }
}