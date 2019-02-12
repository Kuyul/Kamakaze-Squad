using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > GameController.instance.EnemyHealth)
        {
            DestroyEnemy();
        }
    }

    public void DestroyEnemy()
    {
        Instantiate(GameController.instance.peEnemyPop, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
