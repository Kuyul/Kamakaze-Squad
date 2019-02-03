using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            other.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            other.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.tag == "squad")
        {
            collision.gameObject.SetActive(false);
            Destroy(gameObject);
        }

    }
}
