using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //Declare public variables
    [HideInInspector]
    public bool FollowPlayer = true;
    public float TransitionSpeed = 20.0f;

    //Declare private variables
    private Vector3 initialOffset;
    private Vector3 offset;
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
                transform.position = new Vector3(transform.position.x, transform.position.y, Player.transform.position.z - offset.z);
            }
        }
    }

    public void SetNewCameraPos(Vector3 roadPos)
    {
        NewRoadPos = roadPos;
        IsTransitioning = true;
    }
}
