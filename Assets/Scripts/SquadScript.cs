using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadScript : MonoBehaviour
{
    //Declare public variables
    public GameObject Beanie;
    public GameObject Vest;
    public GameObject Dynamite;
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
            //GameController.instance.RemoveSquad(gameObject);
        }

        if (other.tag == "player")
        {
            Beanie.SetActive(true);
            Vest.SetActive(true);
            Dynamite.SetActive(true);
            peSquaded.SetActive(true);
            Instantiate(GameController.instance.peSquadedSplash, transform.position, Quaternion.identity);
            anim.SetTrigger("run");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "block")
        {
           // collision.gameObject.SetActive(false);
            GameController.instance.Detonate(transform.position);
            gameObject.SetActive(false);
            GameObject temp = Instantiate(GameController.instance.peBlock, collision.transform.position, Quaternion.identity);
            Destroy(temp, 5f);
            GameObject temp2 = Instantiate(GameController.instance.peExplosion, transform.position, Quaternion.identity);
            Destroy(temp2, 5f);
        }
    }    
}
