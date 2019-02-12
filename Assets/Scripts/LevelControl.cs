using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelControl : MonoBehaviour
{
    public static LevelControl instance;

    //Declare Level Gameobjects
    public LevelScript[] NormalLevels;
    public LevelScript[] BossLevels;

    //Declare public variables
    public GameObject Road;
    public float RoadZOffset = 300.0f;
    public float RoadYOffset = -30.0f;
    public float LevelLength = 150.0f;
    public int NumberOfTries = 3;
    public int NumberOfRoundsPerLevel = 5;
    public GameObject FinishLinePrefab;
    public float BuildingDistance = 10.0f;
    public float EndblockDistance = 3.0f;
    public GameObject EndBlock;
    public GameObject[] SquadPrefabs;
    public GameObject Obstacle;
    public float ObstacleAngularVel = 60.0f;
    public float SquadXAxisRange = 3.0f;
    public GameObject ExplosionZone;
    public int NumObstacleZPos = 10;
    public float FirstObstacleDistance = 30.0f;
    public int ObstacleXAxisDiv = 2; //From -2 to 2

    [HideInInspector]
    public bool finishLinePassed = false;
    public List<LevelScript> InstantiatedLevels = new List<LevelScript>();

    //Declare private variables
    private List<BuildingScript> SpawnedBuildings = new List<BuildingScript>();
    private GameObject Bomb;
    private int LevelsPassed = 0;

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
        for(int i = 0; i < NumberOfRoundsPerLevel; i++)
        {
            SpawnRounds(i);
        }

        // set first stage of dot dot dot to black
        GameController.instance.levelImages[0].GetComponent<Image>().color = new Color32(50,50,50,255);
    }

    private void SpawnRounds(int level)
    {
        LevelScript currentLevel = NormalLevels[Random.Range(0, NormalLevels.Length)];
        if (level == NumberOfRoundsPerLevel - 1) //Boss level
        {
            currentLevel = Instantiate(BossLevels[Random.Range(0, BossLevels.Length)]);
        }
        else
        {
            currentLevel = Instantiate(NormalLevels[Random.Range(0, NormalLevels.Length)]);
        }
        InstantiatedLevels.Add(currentLevel);

        //Generate Road
        var roadPos = new Vector3(0, RoadYOffset * level, RoadZOffset * level);
        Instantiate(Road, roadPos, Quaternion.identity);

        //Generate finishline
        var finishLinePos = new Vector3(0, 0, LevelLength);
        Instantiate(FinishLinePrefab, finishLinePos + roadPos, Quaternion.identity);
        SquadRandomiser(roadPos, currentLevel); //Spawn Squads
        PlaceObstacles(roadPos, currentLevel); //Spawn Obstacles

        //Check whether the level is a new level or a continued level. If its a new level, we'll spawn a brand new building, if its continued,
        //LevelContinue object will hold reference to the state of the building prior to continuing, so it'll begin from there.
        GameObject spawnedBuilding;
        if (!LevelContinue.instance.levelIsContinued)
        {
            spawnedBuilding = Instantiate(currentLevel.Building, roadPos + finishLinePos + new Vector3(0, 0.6f, BuildingDistance), Quaternion.identity);
        }
        else
        {
            spawnedBuilding = LevelContinue.instance.Building;
        }
        SpawnedBuildings.Add(spawnedBuilding.GetComponent<BuildingScript>());

        //Instantiate an explosion zone below the building
        ExplosionZone.transform.localScale = new Vector3(100f, 1.5f, 100f);
        Instantiate(ExplosionZone, spawnedBuilding.transform.position, Quaternion.identity);

        Instantiate(EndBlock, spawnedBuilding.transform.position + new Vector3(0, 0, EndblockDistance), Quaternion.identity);
    }

    public int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt("Level", 1);
    }

    //This method determines whether the level should continue or whether we should begin a new level.
    //When we see that the bomb is still active on the buidling (means game is still not over), then we check
    //whether there are more tries left, if so, continue on with the current level while keeping the building where it was left off from this level.
    public void ContinueLevel()
    {
        InstantiatedLevels[LevelsPassed].NumberOfTries--;
        if (InstantiatedLevels[LevelsPassed].NumberOfTries == 0)
        {
            LevelFail();
        }
        else
        {
            var roadPos = GetCurrentRoadPosition();
            InstantiatedLevels[LevelsPassed].ResetLevel();
            SquadRandomiser(roadPos, InstantiatedLevels[LevelsPassed]);
            PlaceObstacles(roadPos, InstantiatedLevels[LevelsPassed]);
            GameController.instance.SetNewCameraPosition(roadPos);
        }
    }

    public void LevelFail()
    {
        StartCoroutine(DeathPanelDelay(1f));
    }

    IEnumerator DeathPanelDelay(float t)
    {
        yield return new WaitForSeconds(t);
        GameController.instance.GameOver();
        GameController.instance.DeathPanel.SetActive(true);
    }

    //If level passed count is less than number of rounds per level, then do not reset the level
    public void LevelClear()
    {
        LevelsPassed++;

        // changes color of dot dot dot image everytime level is passed
        if (NumberOfRoundsPerLevel>LevelsPassed)
        {
            GameController.instance.levelImages[LevelsPassed].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
        }
        //Below if statement is true if its boss level
        if (LevelsPassed >= NumberOfRoundsPerLevel)
        {
            GameController.instance.IncrementLevel();
            GameController.instance.RestartGame();
        }
        else
        {
            var nextRoadPos = GetCurrentRoadPosition();
            GameController.instance.SetNewCameraPosition(nextRoadPos);
        }
    }

    //Called internally and from gamecontroller
    public Vector3 GetCurrentRoadPosition()
    {
        return LevelsPassed * new Vector3(0, RoadYOffset, RoadZOffset);
    }

    //This is called when the player hits current levels' building.
    public void SetBuildingFlag()
    {
        //Set the flag to true so that the game may begin counting down
        SpawnedBuildings[LevelsPassed].flag = true;
    }

    //Randomises the squad groups on the map, the total squad count must be greater than the designated number
    private void SquadRandomiser(Vector3 roadPos, LevelScript currentLevel)
    {
        int[] a = { 0, 1, 2, 3 };
        List<int> alloc = new List<int>();
        int total = 0;
        var numAllocationSpots = LevelLength / currentLevel.MinSquadDistance - 1;
        while (total < currentLevel.NumberOfSquads)
        {
            total = 0;
            alloc.Clear();
            for (int i = 0; i < numAllocationSpots; i++)
            {
                var value = a[Random.Range(0, a.Length)];
                total += value;
                alloc.Add(value);
                if(total >= currentLevel.NumberOfSquads)
                {
                    break;
                }
            }
        }

        //Once the squad counts are allocated, run a loop spawning them
        float zOffset = 0;
        for (int i = 0; i < alloc.Count; i++)
        {
            zOffset += currentLevel.MinSquadDistance;
            if (alloc[i] > 0)
            {
                var squadObj = SquadPrefabs[alloc[i] - 1]; //Squad prefabs must be organised in an incremental manner in the inspector
                var xPos = Random.Range(-SquadXAxisRange, SquadXAxisRange);
                var obj = Instantiate(squadObj, roadPos + new Vector3(xPos, 0,zOffset), Quaternion.Euler(0,180,0));
                currentLevel.Squads.Add(obj);
            }
        }
    }

    private void PlaceObstacles(Vector3 roadPos, LevelScript currentLevel)
    {
        List<int> zPosList = new List<int>(); //This list is a unique list of obstacle indexes generated at random
        var zPosDiv = (LevelLength - FirstObstacleDistance) / NumObstacleZPos;

        for(int i = 0; i < currentLevel.NumberOfObstacles; i++)
        {
            var z = Random.Range(0, NumObstacleZPos);
            while (zPosList.Contains(z))
            {
                z = Random.Range(0, NumObstacleZPos);
            }
            zPosList.Add(z);
        }

        for(int i = 0; i < currentLevel.NumberOfObstacles; i++)
        {
            var xPos = Random.Range(-ObstacleXAxisDiv, ObstacleXAxisDiv); //Obstacles share the same X range with squads
            var zPos = FirstObstacleDistance + zPosDiv * zPosList[i];
            var pos = roadPos + new Vector3(xPos * 1.8f, 3.8f, zPos);
            //Check if given position overlaps with any other gameobjects in the scene
            Collider[] colliders = Physics.OverlapSphere(pos, 1); //2 is Yoffset, 1 is radius of the obstacle.. I know hardcoding is not good, but I think we can get away with it here :)

            //Re-calculate position until there is no overlap
            while(colliders.Length > 0)
            {
                xPos = Random.Range(-ObstacleXAxisDiv, ObstacleXAxisDiv);
                pos = roadPos + new Vector3(xPos * 1.8f, 3.8f, zPos);
                colliders = Physics.OverlapSphere(pos, 1);
            }

            //At this point we found a vacant position
            var obj = Instantiate(Obstacle, pos, Quaternion.identity);
            currentLevel.Obstacles.Add(obj);
            obj.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, ObstacleAngularVel, 0);
        }
    }
}
