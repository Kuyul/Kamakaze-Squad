using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "road")
        {
            Instantiate(GameController.instance.peBigBomb, transform.position, Quaternion.identity);
            LevelControl.instance.BombFall();
        }
    }
}
