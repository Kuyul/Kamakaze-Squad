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
        var building = Buildings[Random.Range(0,Buildings.Length)];
        SpawnedBuilding = Instantiate(building, finishLinePos + new Vector3(0, 0, BuildingDistance), Quaternion.identity);
        Bomb = SpawnedBuilding.GetComponent<BuildingScript>().Bomb;
        Instantiate(EndBlock, SpawnedBuilding.transform.position + new Vector3(0, 0, EndblockDistance), Quaternion.identity);
    }

    private void Update()
    {
        if (finishLinePassed)
        {
            if(GameController.instance.GetSquadCount() == 0)
            {
                Debug.Log(Bomb.GetComponent<Rigidbody>().velocity);
                //If bomb velocity is 0, restart level.. well not really, we have to restart the level while keeping the building shape as is.
                if(Bomb.GetComponent<Rigidbody>().velocity == Vector3.zero)
                {
                    GameController.instance.GameOver();
                }
            }
        }
    }

    public void BombFall()
    {
        //We need to later change this to level complete method, just for now.
        GameController.instance.GameOver();
        SpawnedBuilding.GetComponent<BuildingScript>().DestroyCubes();
    }
}
