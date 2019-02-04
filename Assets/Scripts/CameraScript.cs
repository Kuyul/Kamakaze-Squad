using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //Declare public variables
    [HideInInspector]
    public bool FollowPlayer = true;

    //Declare private variables
    private Vector3 offset;
    private Transform Player;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameController.instance.Player.transform;
        offset = Player.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (FollowPlayer)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Player.transform.position.z - offset.z);
        }
    }
}
