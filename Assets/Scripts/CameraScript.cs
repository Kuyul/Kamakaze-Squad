using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform Player;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = Player.position - transform.position;    
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, Player.position.z - offset.z);
    }
}
