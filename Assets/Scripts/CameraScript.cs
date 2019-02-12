using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //Declare public variables
    
    public bool FollowPlayer;
    public float TransitionSpeed = 20.0f;
    public float ySpeed;
    public float zSpeed;

    //Declare private variables
    private Vector3 initialOffset;
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
            ResetyOffset();
            transform.position = Vector3.MoveTowards(transform.position, NewRoadPos + initialOffset, TransitionSpeed * Time.deltaTime);
            if(transform.position == NewRoadPos + initialOffset)
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
    }

    public void ResetyOffset()
    {
        yOffset = 0;
    }
}
