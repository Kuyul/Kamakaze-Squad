using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float Health=3f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > Health)
        {
            LevelContinue.instance.IncrementEnemyCount();
            Destroy(gameObject);
        }
    }
}
