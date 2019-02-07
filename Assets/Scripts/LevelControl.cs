using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    public static LevelControl instance;

    //Declare Level Gameobjects
    public LevelScript[] Levels;

    //Declare public variables
    public int NumberOfTries = 3;
    public GameObject FinishLinePrefab;
    public float BuildingDistance = 10.0f;
    public float EndblockDistance = 3.0f;
    public GameObject EndBlock;
    public GameObject[] SquadPrefabs;
    public float SquadXAxisRange = 3.0f;
    public GameObject Obstacle;
    public float ObstacleAngularVel = 60.0f;

    [HideInInspector]
    public bool finishLinePassed = false;

    //Declare private variables
    private LevelScript CurrentLevel;
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
        //TODO retrieve all level information from LevelScript
        CurrentLevel = Levels[GetCurrentLevel() - 1];

        //Generate finishline
        var finishLinePos = new Vector3(0,0,CurrentLevel.LevelLength);
        Instantiate(FinishLinePrefab, finishLinePos, Quaternion.identity);
        SquadRandomiser(); //Spawn Squads
        PlaceObstacles(); //Spawn Obstacles

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
        Instantiate(EndBlock, SpawnedBuilding.transform.position + new Vector3(0, 0, EndblockDistance), Quaternion.identity);
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
            LevelFail();
        }
        else
        {
            DontDestroyOnLoad(LevelContinue.instance.Building);
            LevelContinue.instance.TriesLeft--;
            Debug.Log("Tries Left: " + LevelContinue.instance.TriesLeft);
            LevelContinue.instance.levelIsContinued = true;
            GameController.instance.GameOver();
            Debug.Log("Game Continued");
        }
       
    }

    //Called from the Bomb script to let levelhandler know that the bomb has fallen.
    //TODO: Bomb will detonate instantly right now, but we might want to give it a little bit of time before it destroys the cubes from the scene
    public void BombFall()
    {
        BombLanded = true;
        LevelClear();
    }

    public void LevelFail()
    {
        LevelContinue.instance.ResetLevel();
        GameController.instance.GameOver();
        Debug.Log("Game Over");
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
                Instantiate(squadObj, new Vector3(xPos, 0,zOffset), Quaternion.Euler(0,180,0));
            }
        }
    }

    private void PlaceObstacles()
    {
        for(int i = 0; i < CurrentLevel.NumberOfObstacles; i++)
        {
            var xPos = Random.Range(-SquadXAxisRange, SquadXAxisRange); //Obstacles share the same X range with squads
            var zPos = Random.Range(10.0f, CurrentLevel.LevelLength - 10.0f); //10 here is the offset
            var pos = new Vector3(xPos, 3.8f, zPos);
            //Check if given position overlaps with any other gameobjects in the scene
            Collider[] colliders = Physics.OverlapSphere(pos, 1); //2 is Yoffset, 1 is radius of the obstacle.. I know hardcoding is not good, but I think we can get away with it here :)

            //Re-calculate position until there is no overlap
            while(colliders.Length > 0)
            {
                xPos = Random.Range(-SquadXAxisRange, SquadXAxisRange);
                zPos = Random.Range(20.0f, CurrentLevel.LevelLength - 10.0f);
                pos = new Vector3(xPos, 3.8f, zPos);
                colliders = Physics.OverlapSphere(pos, 1);
            }

            //At this point we found a vacant position
            var obj = Instantiate(Obstacle, pos, Quaternion.identity);
            obj.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, ObstacleAngularVel, 0);
        }
    }
}
