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
        GameObject temp2 = Instantiate(GameController.instance.textMeshEnemy, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity);
        Destroy(temp2, 2f);
        GameController.instance.IncrementCurrentscore(10);
        Instantiate(GameController.instance.peEnemyPop, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
