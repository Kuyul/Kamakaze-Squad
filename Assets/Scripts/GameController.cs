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
    public Player PlayerScript;

    public GameObject Player;
    public float PlayerSpeed;
    public float PlayerMaxSpeed;

    public GameObject pePlayerPop;
    public GameObject peSquadedSplash;
    public GameObject peEnemyPop;

    public GameObject highscoreGO;  
    public GameObject currentscoreGO;
    public GameObject swipetoplayGO;

    public Text HighscoreText;
    public Text CurrentscoreText;
    public Text CurrentLevelText;
    public Text NextLevelText;

    public bool GameStarted;

    public float ExplosionPower = 3.0f;
    public float ExplosionRadius = 5.0f;
    public float ExplosionUpForce = 1.0f;
    public ForceMode expForceMode;

    //Declare private variables


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
        HighscoreText.text = PlayerPrefs.GetInt("highscore", 0).ToString();
        CurrentscoreText.text = PlayerPrefs.GetInt("currentscore", 0).ToString();
        CurrentLevelText.text = PlayerPrefs.GetInt("currentlevel",1).ToString();
        NextLevelText.text = PlayerPrefs.GetInt("nextlevel", PlayerPrefs.GetInt("currentlevel",1)+1).ToString();
    }

    //Called from Player class to stop camera movement when player reaches the finishline
    public void StopCamera()
    {
        CameraScript.FollowPlayer = false;
    }

    public void ActivateCamera()
    {
        CameraScript.FollowPlayer = true;
    }

    public void SetNewCameraPosition(Vector3 roadPos)
    {
        CameraScript.SetNewCameraPos(roadPos);
    }

    //GameOver
    public void GameOver()
    {
        PlayerPrefs.SetInt("currentscore", 0);
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

    public void SwipeToPlay()
    {
        PlayerScript.SetPlayerVelocity();
        swipetoplayGO.SetActive(false);
        highscoreGO.SetActive(false);
        currentscoreGO.SetActive(true);
    }

    // update score by 1 everytime this function is called
    public void IncrementCurrentscore()
    {
        PlayerPrefs.SetInt("currentscore", PlayerPrefs.GetInt("currentscore", 0) + 1);
        CurrentscoreText.text = PlayerPrefs.GetInt("currentscore").ToString();

        // update highscore if current score is greater then highscore
        if (PlayerPrefs.GetInt("currentscore") >= PlayerPrefs.GetInt("highscore", 0))
        {
            PlayerPrefs.SetInt("highscore", PlayerPrefs.GetInt("currentscore"));
        }
    }

    public void IncrementLevel()
    {
        PlayerPrefs.SetInt("currentlevel", PlayerPrefs.GetInt("currentlevel", 1) + 1);
        PlayerPrefs.SetInt("nextlevel", PlayerPrefs.GetInt("currentlevel", 1) + 1);

        CurrentLevelText.text = PlayerPrefs.GetInt("currentlevel", 1).ToString();
        NextLevelText.text = PlayerPrefs.GetInt("nextlevel", PlayerPrefs.GetInt("currentlevel", 1) + 1).ToString();
    }

    //Called from camera script signalling transitioning is complete
    //At this point we re-activate the player
    public void TransitioningComplete()
    {
        Player.transform.position = LevelControl.instance.GetCurrentRoadPosition() + new Vector3(0,0,-15f);
        PlayerScript.EnablePlayer();
        CameraScript.FollowPlayer = true;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("currentscore", 0);
    }
}