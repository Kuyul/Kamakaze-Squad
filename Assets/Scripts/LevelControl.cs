using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    public static LevelControl instance;

    //Declare public variables
    public float DistanceToFinishLine = 50.0f;
    public GameObject FinishLinePrefab;
    public float BuildingDistance = 10.0f;
    public GameObject[] Buildings;

    //Declare private variables

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

        //Spawn a random building...? maybe it shouldn't be random, but for now we'll make it random.
        var building = Buildings[Random.Range(0,Buildings.Length)];
        Instantiate(building, finishLinePos + new Vector3(0, 0, BuildingDistance), Quaternion.identity);
    }

    public void BombFall()
    {
        //We need to later change this to level complete method, just for now.
        GameController.instance.GameOver();
    }
}
