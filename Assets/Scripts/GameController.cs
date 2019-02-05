using System.Collections;
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
    public GameObject peSquad;
    public GameObject peExplosion;
    public GameObject peBigBomb;

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
        yield return new WaitForSeconds(1.0f);
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
}
