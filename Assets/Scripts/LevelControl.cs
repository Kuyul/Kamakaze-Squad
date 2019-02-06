using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    public static LevelControl instance;

    //Declare Level Gameobjects
    public LevelScript[] Levels;

    //Declare public variables
    public GameObject Mother;
    public GameObject Road;
    public int NumberOfTries = 3;
    public GameObject FinishLinePrefab;
    public float BuildingDistance = 10.0f;
    public float EndblockDistance = 3.0f;
    public GameObject EndBlock;
    public GameObject[] SquadPrefabs;
    public float SquadXAxisRange = 3.0f;
    public float LevelRotationSpeed = 90.0f;

    [HideInInspector]
    public bool finishLinePassed = false;

    //Declare private variables
    private LevelScript CurrentLevel;
    private GameObject SpawnedBuilding;
    private GameObject Bomb;
    private bool BombLanded = false;
    private GameObject Center;
    private float MotherAngle = 0;
    private int NumberOfRotates = 0;
    private GameObject RotateAroundObj;

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
        //TODO retrieve all level information from LevelScript
        CurrentLevel = Levels[GetCurrentLevel() - 1];

        //Generate finishline
        var finishLinePos = new Vector3(0,0,CurrentLevel.LevelLength);
        var finishlineobj = Instantiate(FinishLinePrefab, finishLinePos, Quaternion.identity);
        finishlineobj.transform.SetParent(Mother.transform);
        SquadRandomiser();
        //Spawn Squads

        //Spawn a random building...? maybe it shouldn't be random, but for now we'll make it random.
        //Check whether the level is a new level or a continued level. If its a new level, we'll spawn a brand new building, if its continued,
        //LevelContinue object will hold reference to the state of the building prior to continuing, so it'll begin from there.
        if (!LevelContinue.instance.levelIsContinued)
        {
            SpawnedBuilding = Instantiate(CurrentLevel.Building, finishLinePos + new Vector3(0, 0, BuildingDistance), Quaternion.identity);
            LevelContinue.instance.Building = SpawnedBuilding;
            LevelContinue.instance.TriesLeft = CurrentLevel.NumberOfTries;
        }
        else
        {
            SpawnedBuilding = LevelContinue.instance.Building;
        }
        Bomb = SpawnedBuilding.GetComponent<BuildingScript>().Bomb;
        SpawnedBuilding.transform.SetParent(Mother.transform);
        var endblockObj = Instantiate(EndBlock, SpawnedBuilding.transform.position + new Vector3(0, 0, EndblockDistance), Quaternion.identity);
        endblockObj.transform.SetParent(Mother.transform);
        RoadGeneration();
    }

    void Update()
    {
        if (RotateAroundObj != null)
        {
            var step = LevelRotationSpeed * Time.deltaTime;
            RotateAroundObj.transform.rotation = Quaternion.RotateTowards(RotateAroundObj.transform.rotation, Quaternion.Euler(0, MotherAngle, 0), step);
        }
    }

    public int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt("Level", 1);
    }

    //This method determines whether the level should continue or whether we should begin a new level.
    //When we see that the bomb is still active on the buidling (means game is still not over), then we check
    //whether there are more tries left, if so, continue on with the current level while keeping the building where it was left off from this level.
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

    //Called from the Bomb script to let levelhandler know that the bomb has fallen.
    //TODO: Bomb will detonate instantly right now, but we might want to give it a little bit of time before it destroys the cubes from the scene
    public void BombFall()
    {
        BombLanded = true;
        LevelClear();
    }

    private void LevelClear()
    {
        var newLvl = GetCurrentLevel() + 1;
        if(newLvl > Levels.Length)
        {
            newLvl = 1;
        }
        PlayerPrefs.SetInt("Level", newLvl);
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

    //Randomises the squad groups on the map, the total squad count must be greater than the designated number
    private void SquadRandomiser()
    {
        int[] a = { 0, 1, 2, 3 };
        List<int> alloc = new List<int>();
        int total = 0;
        var numAllocationSpots = CurrentLevel.LevelLength / CurrentLevel.MinSquadDistance - 1;
        while (total < CurrentLevel.NumberOfSquads)
        {
            total = 0;
            alloc.Clear();
            for (int i = 0; i < numAllocationSpots; i++)
            {
                var value = a[Random.Range(0, a.Length)];
                total += value;
                alloc.Add(value);
                if(total >= CurrentLevel.NumberOfSquads)
                {
                    break;
                }
            }
        }

        //Once the squad counts are allocated, run a loop spawning them
        float zOffset = 0;
        for (int i = 0; i < alloc.Count; i++)
        {
            zOffset += CurrentLevel.MinSquadDistance;
            if (alloc[i] > 0)
            {
                var squadObj = SquadPrefabs[alloc[i] - 1]; //Squad prefabs must be organised in an incremental manner in the inspector
                var xPos = Random.Range(-SquadXAxisRange, SquadXAxisRange);
                var obj = Instantiate(squadObj, new Vector3(xPos, 0,zOffset), Quaternion.Euler(0,180,0));
                obj.transform.parent = Mother.transform;
            }
        }
    }

    private void RoadGeneration()
    {
        //Complex algorithm. Hopefully I don't have to touch this part of the code again ;)
        var div = CurrentLevel.LevelLength / CurrentLevel.NumberOfRoads;
        Road.transform.localScale = new Vector3(Road.transform.localScale.x, Road.transform.localScale.y, div);
        float xPos = 0.0f;
        float zPos = div/2;
        float yAngle = 0;
        for(int i = 0; i < CurrentLevel.NumberOfRoads; i++)
        {
            var position = new Vector3(xPos, 0, zPos);
            var angle = Quaternion.Euler(0, yAngle, 0);
            var obj = Instantiate(Road, position, angle);
            obj.transform.parent = Mother.transform;
            xPos = xPos - Road.transform.localScale.x / 2 + Road.transform.localScale.z / 2;
            zPos = zPos - Road.transform.localScale.x / 2 + Road.transform.localScale.z / 2;
            yAngle += 90 * Mathf.Pow(-1, i);
        }
    }

    public void MotherRotationChange(GameObject dummyObj)
    {
        if(NumberOfRotates > 0)
        {
            MotherAngle = Mathf.Pow(-1, NumberOfRotates) * 90;
            RotateAroundObj = dummyObj;
            Mother.transform.parent = RotateAroundObj.transform;
        }
        NumberOfRotates++;
    }
}
