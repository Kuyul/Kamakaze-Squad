using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.tag == "squad")
        {
            collision.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }

    }
}
