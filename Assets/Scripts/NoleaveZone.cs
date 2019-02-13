using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoleaveZone : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "block")
        {
            Destroy(other.gameObject);
            Instantiate(GameController.instance.peBox, other.transform.position, Quaternion.identity);
        }
        if(other.tag == "enemy")
        {
            other.GetComponent<Enemy>().DestroyEnemy();
        }
    }
}
