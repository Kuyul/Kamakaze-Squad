﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    //Declare public variables
    public CameraScript CameraScript;
    public GameObject Player;
    public float PlayerSpeed;

    public GameObject peBlock;
    public GameObject peSquadedSplash;
    public GameObject peExplosion;
    public GameObject peBigBomb;

    public float ExplosionPower = 3.0f;
    public float ExplosionRadius = 5.0f;
    public float ExplosionUpForce = 1.0f;
    public ForceMode expForceMode;

    //Declare private variables
    private Player PlayerScript;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerScript = Player.GetComponent<Player>();
        InvokeRepeating("IncreaseSpeed", 1f, 1f);
    }

    // increase speed through invoke method every x seconds
    public void IncreaseSpeed()
    {
        PlayerSpeed += 0.5f;
        PlayerScript.UpdateSpeed();

        if (PlayerSpeed >= 10)
        {
            CancelInvoke("IncreaseSpeed");
        }
    }

    // called when you hit sign to reset velocity
    public void ResetSpeed()
    {
        CancelInvoke();
        PlayerSpeed = 1;
        InvokeRepeating("IncreaseSpeed", 0f, 1f);
    }

    //Called from Player class to stop camera movement when player reaches the finishline
    public void StopCamera()
    {
        CameraScript.FollowPlayer = false;
    }

    //GameOver
    public void GameOver()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(0);
    }

    public void RemoveSquad(GameObject obj)
    {
        PlayerScript.RemoveSquad(obj);
    }

    //Called from Levelcontrol class??
    public int GetSquadCount()
    {
        return PlayerScript.GetSquadCount();
    }

    //Called from the player class and squad class when it collides with a block.
    //It'll add force to the surrounding blocks to give an explosion effect
    public void Detonate(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, ExplosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.tag == "block")
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                rb.AddExplosionForce(ExplosionPower, pos, ExplosionRadius, ExplosionUpForce, expForceMode);
            }
        }
    }
}