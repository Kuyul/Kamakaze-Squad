using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    //Declare public variables
    public Player PlayerScript;
    public float PlayerSpeed;

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
}
