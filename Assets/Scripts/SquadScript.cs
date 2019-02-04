using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadScript : MonoBehaviour
{
    //Declare public variables
    public GameObject Beanie;
    public GameObject Vest;

    //Declare private variables
    private Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "endblock")
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "block")
        {
            Destroy(collision.gameObject);
            GameController.instance.RemoveSquad(gameObject);
            gameObject.SetActive(false);
        }
    }
}
