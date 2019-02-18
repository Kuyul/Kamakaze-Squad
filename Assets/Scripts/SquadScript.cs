using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadScript : MonoBehaviour
{
    //Declare public variables
    public GameObject peSquaded;
    public GameObject peRunFever;
    public GameObject peRun;
    public GameObject peTrail;
    public GameObject peFever;

    private bool hitPlayer = false;

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
            if (!hitPlayer)
            {
                peSquaded.SetActive(true);
                GameObject temp = Instantiate(GameController.instance.peSquadedSplash, transform.position, Quaternion.identity);
                Destroy(temp, 1f);
                GameObject temp2 = Instantiate(GameController.instance.textMesh, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity);
                Destroy(temp2, 1.5f);
                anim.SetTrigger("run");
                GameController.instance.IncrementCurrentscore(1);
                LevelControl.instance.DecrementSquadCount();
                hitPlayer = true;
            }
        }

        if (other.tag == "block")
        {
            GameController.instance.Detonate(transform.position);
            gameObject.SetActive(false);
            GameObject temp3 = Instantiate(GameController.instance.pePlayerPop, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity);
            Destroy(temp3, 5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "block")
        {
            GameController.instance.Detonate(transform.position);
            gameObject.SetActive(false);
            GameObject temp3 = Instantiate(GameController.instance.pePlayerPop, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity);
            Destroy(temp3, 5f);
        }
    }    

}
