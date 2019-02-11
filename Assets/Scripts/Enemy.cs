using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float Health = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > Health)
        {
            DestroyEnemy();
        }
    }

    public void DestroyEnemy()
    {
        LevelContinue.instance.IncrementEnemyCount();
        Instantiate(GameController.instance.peEnemyPop, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
