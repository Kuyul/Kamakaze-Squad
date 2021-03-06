﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //Declare public variables

    public bool CamMoveUp;
    public bool FollowPlayer;
    public bool FOVMove;
    public float TransitionSpeed = 20.0f;
    public float ySpeed;
    public float FOVspeed;

    //Declare private variables
    public Vector3 initialOffset;
    private Vector3 offset;
    private float yOffset = 0;
    private Transform Player;

    //New position variables
    private Vector3 NewRoadPos;
    private bool IsTransitioning;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameController.instance.Player.transform;
        initialOffset = transform.position; //offset from 0,0,0
        offset = Player.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsTransitioning)
        {
            if (Camera.main.fieldOfView > 45)
            {
                Camera.main.fieldOfView -= Time.deltaTime * FOVspeed;
            }

            ResetyOffset();
            transform.position = Vector3.MoveTowards(transform.position, NewRoadPos + initialOffset, TransitionSpeed * Time.deltaTime);
            if (transform.position == NewRoadPos + initialOffset)
            {
                IsTransitioning = false;
                GameController.instance.TransitioningComplete();
            }
        }
        else
        {
            if (FollowPlayer)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * yOffset, Player.transform.position.z - offset.z);
            }

            else if(CamMoveUp)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * yOffset, transform.position.z);
                if (FOVMove)
                {
                    Camera.main.fieldOfView += Time.deltaTime * FOVspeed;
                }
            }
        }
    }

    public void SetNewCameraPos(Vector3 roadPos)
    {
        NewRoadPos = roadPos;
        IsTransitioning = true;
    }

    public void MoveCam()
    {
        FollowPlayer = true;
    }

    public void SetyOffset()
    {
        yOffset = ySpeed;
        FollowPlayer = false;
        CamMoveUp = true;
        FOVMove = true;
    }
     
    public void ResetyOffset()
    {
        yOffset = 0;
        FollowPlayer = true;
    } 
}