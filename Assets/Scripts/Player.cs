using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //Declare public variables
    public float KeepDistance = 1.0f;

    //Declare private variables
    private Rigidbody rb;
    private List<GameObject> ListOfSquads = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.forward * GameController.instance.PlayerSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ListOfSquads.Count; i++)
        {
            if (transform.position.z - ListOfSquads[i].transform.position.z >= KeepDistance)
            {
                var a = transform.position - ListOfSquads[i].transform.position;
                ListOfSquads[i].gameObject.GetComponent<Rigidbody>().velocity = a * 6f;
            }
            else
            {
                ListOfSquads[i].gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    public void UpdateSpeed()
    {
        rb.velocity = Vector3.forward * GameController.instance.PlayerSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "squad")
        {
            ListOfSquads.Add(other.gameObject);
            //other.gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
            StartCoroutine(Delay(other));
        }
    }

    IEnumerator Delay(Collider other)
    {
        yield return new WaitForSeconds(0.2f);
        other.gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
    }
}
