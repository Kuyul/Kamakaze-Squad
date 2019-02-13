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
        }
        if(other.tag == "enemy")
        {
            other.GetComponent<Enemy>().DestroyEnemy();
        }
    }
}
