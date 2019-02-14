using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinCrown : MonoBehaviour
{
    public Transform player;
    public float yHeight;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();    
    }

    // Update is called once per frame
    void Update()
    {
        rb.angularVelocity = new Vector3(0,GameController.instance.CrownSpinSpeed,0);
        transform.position = new Vector3(player.position.x, player.position.y+yHeight, player.position.z);
    }
}
