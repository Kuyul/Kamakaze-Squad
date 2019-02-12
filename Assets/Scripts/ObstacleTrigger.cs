using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTrigger : MonoBehaviour
{
    public PopupObstacle Obstacle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            Obstacle.Triggered = true;
        }
    }
}
