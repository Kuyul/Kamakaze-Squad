using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupObstacle : MonoBehaviour
{
    public float MoveSpeed = 2.0f;
    public float MoveTowardsYPos;
    public bool Triggered = false; //set to true from obstacle trigger script

    private Vector3 MoveToPosition;

    private void Start()
    {
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, LevelControl.instance.ObstacleAngularVel, 0);
        MoveToPosition = new Vector3(transform.position.x, transform.position.y + MoveTowardsYPos, transform.position.z);
    }

    void Update()
    {
        if (Triggered)
        {
            transform.position = Vector3.MoveTowards(transform.position, MoveToPosition, MoveSpeed * Time.deltaTime);
        }
    }
}
