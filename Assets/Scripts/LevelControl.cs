using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    public static LevelControl instance;

    //Declare public variables
    public int NumberOfTries = 3;
    public float DistanceToFinishLine = 50.0f;
    public GameObject FinishLinePrefab;
    public float BuildingDistance = 10.0f;
    public GameObject[] Buildings;
    public float EndblockDistance = 3.0f;
    public GameObject EndBlock;
    public int NoOfSquads = 5;
    public float MinimumDistanceApart = 8.0f;
    public GameObject[] SquadPrefabs;

    [HideInInspector]
    public bool finishLinePassed = false;

    //Declare private variables
    private GameObject SpawnedBuilding;
    private GameObject Bomb;
    private bool BombLanded = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    //Initialise level
    void Start()
    {
        //Generate finishline
        var finishLinePos = new Vector3(0,0,DistanceToFinishLine);
        Instantiate(FinishLinePrefab, finishLinePos, Quaternion.identity);

        //Spawn Squads

        //Spawn a random building...? maybe it shouldn't be random, but for now we'll make it random.
        //Check whether the level is a new level or a continued level. If its a new level, we'll spawn a brand new building, if its continued,
        //LevelContinue object will hold reference to the state of the building prior to continuing, so it'll begin from there.
        if (!LevelContinue.instance.levelIsContinued)
        {
            var building = Buildings[Random.Range(0, Buildings.Length)];
            SpawnedBuilding = Instantiate(building, finishLinePos + new Vector3(0, 0, BuildingDistance), Quaternion.identity);
            LevelContinue.instance.Building = SpawnedBuilding;
            LevelContinue.instance.TriesLeft = NumberOfTries;
        }
        else
        {
            SpawnedBuilding = LevelContinue.instance.Building;
        }
        Bomb = SpawnedBuilding.GetComponent<BuildingScript>().Bomb;
        Instantiate(EndBlock, SpawnedBuilding.transform.position + new Vector3(0, 0, EndblockDistance), Quaternion.identity);
    }

    private void ContinueLevel()
    {
        if (LevelContinue.instance.TriesLeft - 1 == 0)
        {
            LevelContinue.instance.ResetLevel();
            Debug.Log("Game Over");
        }
        else
        {
            DontDestroyOnLoad(LevelContinue.instance.Building);
            LevelContinue.instance.TriesLeft--;
            Debug.Log("Tries Left: " + LevelContinue.instance.TriesLeft);
            LevelContinue.instance.levelIsContinued = true;
            Debug.Log("Game Continued");
        }
        GameController.instance.GameOver();
    }

    public void BombFall()
    {
        //We need to later change this to level complete method, just for now.
        BombLanded = true;
        GameController.instance.GameOver();
        Debug.Log("Level Clear!");
        LevelContinue.instance.ResetLevel();
        SpawnedBuilding.GetComponent<BuildingScript>().DestroyCubes();
    }

    //Called from the Player script after 3 seconds of player hitting a block or an endblock
    public void CheckBombFallen()
    {
        if (!BombLanded)
        {
            ContinueLevel();
        }
    }
}
