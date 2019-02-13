using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadScript : MonoBehaviour
{
    //Declare public variables
    public GameObject Beanie;
    public GameObject Vest;
    public GameObject peSquaded;
    public GameObject peRun;

    //Declare private variables
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "endblock")
        {
            gameObject.SetActive(false);
        }

        if (other.tag == "player")
        {
            Beanie.SetActive(true);
            Vest.SetActive(true);
            peSquaded.SetActive(true);
            Instantiate(GameController.instance.peSquadedSplash, transform.position, Quaternion.identity);
            anim.SetTrigger("run");
            GameController.instance.IncrementCurrentscore();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "block")
        {
            GameController.instance.Detonate(transform.position);
            gameObject.SetActive(false);
            Instantiate(GameController.instance.pePlayerPop, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity);            
        }
    }    
}
