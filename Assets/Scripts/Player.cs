using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Declare public variables
    public float KeepDistance = 1.0f;
    public float ChaseSpeed = 5.0f;
    public GameObject Mesh;
    public GameObject Beanie;
    public GameObject Vest;
    public GameObject peRun;
    public GameObject peTrail;

    public Touch touchScript;

    //Declare private variables
    private Rigidbody rb;
    private List<GameObject> ListOfSquads = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ListOfSquads.Count; i++)
        {
            if (transform.position.z - ListOfSquads[i].transform.position.z >= KeepDistance)
            {
                var a = transform.position - ListOfSquads[i].transform.position;
                ListOfSquads[i].transform.rotation = transform.rotation;
                ListOfSquads[i].gameObject.GetComponent<Rigidbody>().velocity = a * ChaseSpeed;
            }
            else
            {
                ListOfSquads[i].gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    IEnumerator Delay(Collider other)
    {
        yield return new WaitForSeconds(0.2f);
        other.gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
    }

    //Called when player hits a block or an endblock to disable the relevant components so that player doesn't collide with more blocks and seems like its disabled.
    //We can't disable the whole player because the squads need a reference object to follow
    private void DisablePlayer()
    {
        GetComponent<Collider>().enabled = false;
        Mesh.SetActive(false);
        Beanie.SetActive(false);
        Vest.SetActive(false);     
        peRun.SetActive(false);
        peTrail.SetActive(false);
        
        //StartCoroutine(CheckAfterThreeSeconds());
    }

    //Called from Gamecontroller after a signal from Camerascript saying transitioning is complete
    public void EnablePlayer()
    {
        GetComponent<Collider>().enabled = true;
        Mesh.SetActive(true);
        Beanie.SetActive(true);
        Vest.SetActive(true);
        peRun.SetActive(true);
        peTrail.SetActive(true);
    }

    //Wait for three seconds after player is disabled. See whether the bomb has fallen.
    IEnumerator CheckAfterThreeSeconds()
    {
        yield return new WaitForSeconds(3.0f);
        LevelControl.instance.CheckBombFallen();
    }

    //Remove Squad from list
    public void RemoveSquad(GameObject Squad) {
        ListOfSquads.Remove(Squad);
    }

    //Called from Game Controller 
    public int GetSquadCount()
    {
        return ListOfSquads.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "squad")
        {
            ListOfSquads.Add(other.gameObject);
            StartCoroutine(Delay(other));
        }

        if (other.tag == "block")
        {
            GameController.instance.Detonate(transform.position);
            LevelControl.instance.LevelClear();
            DisablePlayer();
            Instantiate(GameController.instance.pePlayerPop, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity);
        }

        if (other.tag == "endblock")
        {
            DisablePlayer();
        }

        if (other.tag == "finishline")
        {
            rb.velocity = Vector3.forward * GameController.instance.PlayerMaxSpeed;
            GameController.instance.StopCamera();
            LevelControl.instance.finishLinePassed = true;
            peRun.SetActive(true);
            touchScript.IncreaseXLimit();

            for (int i = 0; i < ListOfSquads.Count; i++)
            {
                ListOfSquads[i].GetComponent<SquadScript>().peRun.SetActive(true);
            }
        }

        if (other.tag == "obstacle")
        {
            gameObject.SetActive(false);
            Destroy(other.gameObject);
            LevelControl.instance.LevelFail();
            //  Instantiate(GameController.instance.peBigBomb, other.gameObject.transform.position, Quaternion.identity);
        }
    }

    public void SetPlayerVelocity()
    {
        rb.velocity = Vector3.forward* GameController.instance.PlayerSpeed;        
    }
}
