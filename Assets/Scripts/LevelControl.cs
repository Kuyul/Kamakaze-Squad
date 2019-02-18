using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelControl : MonoBehaviour
{
    public static LevelControl instance;

    //Declare Level Gameobjects
    public LevelScript[] NormalLevels;
    public LevelScript[] EasyBossLevels;
    public LevelScript[] HardBossLevels;

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
    public int NumberOfRoadDivides = 20;
    public int NumObstacleZPos = 10;
    public float FirstObstacleDistance = 30.0f;
    public float ObstacleStartingYPos = 0.0f;
    public int ObstacleXAxisDiv = 2; //From -2 to 2
    public float ObstacleXDistance = 2.0f;
    public int FirstRoundNumObstacles = 3;
    public bool TriggerTutorial = true;
    public GameObject Tutorial;
    public int StartingPoolSize = 5;
    public int PoolIncreasePerLevel = 2;
    public int DefaultSquadCountAdjust = 0;
    public float BuildingScale = 1.5f;
    public bool FeverTimeActive = true;
    public float FeverTimeSlowmoScale = 0.5f;
    public float FeverTimeSlowmoTime = .3f;

    public Color[] BackgroundColors;
    public Material[] EvenMaterials;
    public Material[] OddMaterials;

    [HideInInspector]
    public bool finishLinePassed = false;
    public List<LevelScript> InstantiatedLevels = new List<LevelScript>();

    //Declare private variables
    private List<BuildingScript> SpawnedBuildings = new List<BuildingScript>();
    private List<GameObject> Rounds = new List<GameObject>();
    private List<int> NoOverlapList = new List<int>();
    //private GameObject Bomb;
    private int LevelsPassed = 0;
    private float PlacementDistance;

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
        if (GetCurrentLevel() == 0)
        {
            Tutorial.SetActive(true);
        }
        else
        {
            //Alternating background colors per level
            if (BackgroundColors.Length > 0)
            {
                var colorIndex = (GetCurrentLevel() - 1) % BackgroundColors.Length;
                Camera.main.backgroundColor = BackgroundColors[colorIndex];
            }
            PlacementDistance = (LevelLength - FirstObstacleDistance) / NumberOfRoadDivides;

            for (int i = 0; i < NumberOfRoundsPerLevel; i++)
            {
                SpawnRounds(i);
            }
            ShowRounds();
            // set first stage of dot dot dot to black
            GameController.instance.levelImages[0].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
        }
    }

    private void SpawnRounds(int level)
    {
        //Add rounds so we can control which rounds to show
        GameObject round = new GameObject("Round " + level);
        Rounds.Add(round);

        LevelScript currentLevel;
       
        if (level == NumberOfRoundsPerLevel - 1) //Boss level
        {
            //Boss levels are incrementally spawned, i.e. level 1 = always boss level 1, level 2 = boss level 2... and so on
            if (GetCurrentLevel() <= 7)
            {
                currentLevel = Instantiate(EasyBossLevels[(GetCurrentLevel() - 1) % EasyBossLevels.Length]);
            }
            //Different pool of more challenging boss levels
            else
            {
                var index = (GetCurrentLevel() - 1) % (HardBossLevels.Length/2);
                var bossLevel = Random.Range(1, 3) * (index + 1) - 1; 
                currentLevel = Instantiate(HardBossLevels[bossLevel]);
            }
        }
        else
        {
            var poolSize = StartingPoolSize + GetCurrentLevel() * PoolIncreasePerLevel;
            if(poolSize > NormalLevels.Length)
            {
                poolSize = NormalLevels.Length;
            }

            var index = Random.Range(0, poolSize);
            while (NoOverlapList.Contains(index))
            {
                index = Random.Range(0, poolSize);
            }
            NoOverlapList.Add(index);

            currentLevel = Instantiate(NormalLevels[index]);
        }
        InstantiatedLevels.Add(currentLevel);

        //Generate Road
        var roadPos = new Vector3(0, RoadYOffset * level, RoadZOffset * level);
        var road = Instantiate(Road, roadPos, Quaternion.identity);

        //Generate finishline
        var finishLinePos = new Vector3(0, 0, LevelLength);
        var finishline = Instantiate(FinishLinePrefab, finishLinePos + roadPos, Quaternion.identity);
        finishline.transform.SetParent(round.transform);
        SpawnSquadsObstacles(roadPos, currentLevel, level + FirstRoundNumObstacles, round); //Spawn Squads

        GameObject spawnedBuilding = Instantiate(currentLevel.Building, roadPos + finishLinePos + new Vector3(0, 0.6f, BuildingDistance), Quaternion.identity);
        SpawnedBuildings.Add(spawnedBuilding.GetComponent<BuildingScript>());
        spawnedBuilding.transform.SetParent(round.transform);
        spawnedBuilding.transform.localScale = new Vector3(BuildingScale, BuildingScale, BuildingScale);

        //Instantiate an explosion zone below the building
        ExplosionZone.transform.localScale = new Vector3(100f, 1.5f, 100f);
        var explosionZone = Instantiate(ExplosionZone, spawnedBuilding.transform.position, Quaternion.identity);
        explosionZone.transform.SetParent(road.transform);

        var endBlock = Instantiate(EndBlock, spawnedBuilding.transform.position + new Vector3(0, 0, EndblockDistance), Quaternion.identity);
        endBlock.transform.SetParent(road.transform);
    }

    public int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt("currentlevel", 0);
    }

    //This method determines whether the level should continue or whether we should begin a new level.
    //When we see that the bomb is still active on the buidling (means game is still not over), then we check
    //whether there are more tries left, if so, continue on with the current level while keeping the building where it was left off from this level.
    public void ContinueLevel()
    {
        if (Tutorial.activeInHierarchy)
        {
            LevelFail();
        }
        else
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
                SpawnSquadsObstacles(roadPos, InstantiatedLevels[LevelsPassed], LevelsPassed + FirstRoundNumObstacles, Rounds[LevelsPassed]);
                GameController.instance.SetNewCameraPosition(roadPos);
            }
        }
    }

    public void LevelFail()
    {
        StartCoroutine(DelayDeathPanel(1f));
    }

    IEnumerator DelayDeathPanel(float t)
    {
        yield return new WaitForSeconds(t);
        GameController.instance.GameOver();
        GameController.instance.DeathPanel.SetActive(true);
    }

    //If level passed count is less than number of rounds per level, then do not reset the level
    public void LevelClear()
    {
        if (Tutorial.activeInHierarchy)
        {
            StartCoroutine(DelayLevelClear(0.1f, 0.25f, 2.1f));
        }
        else
        {
            LevelsPassed++;

            // changes color of dot dot dot image everytime level is passed
            if (NumberOfRoundsPerLevel > LevelsPassed)
            {
                GameController.instance.levelImages[LevelsPassed].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            }

            if (NumberOfRoundsPerLevel == LevelsPassed + 1)
            {
                StartCoroutine(DelayBossPanel(0.9f, 0.88f));
            }


            //Below if statement is true if its boss level is cleared
            if (LevelsPassed >= NumberOfRoundsPerLevel)
            {
                StartCoroutine(DelayLevelClear(0.1f, 0.25f, 2.1f));
            }

            // else move camera to next round and continue
            else
            {
                StartCoroutine(DelayCameraTransition(1f));
            }
        }
    }

    IEnumerator DelayLevelClear(float t1,float t2, float t3)
    {
        yield return new WaitForSeconds(t1);
        GameController.instance.peConfettiLeft.SetActive(true);
        GameController.instance.peConfettiRight.SetActive(true);
        yield return new WaitForSeconds(t2);
        GameController.instance.LevelClearPanel.SetActive(true);
        yield return new WaitForSeconds(t3);
        GameController.instance.IncrementLevel();
        GameController.instance.RestartGameInstant();
    }

    IEnumerator DelayCameraTransition(float t)
    {
        yield return new WaitForSeconds(t);
        var nextRoadPos = GetCurrentRoadPosition();
        GameController.instance.SetNewCameraPosition(nextRoadPos);
    }

    IEnumerator DelayBossPanel(float t1,float t2)
    {
        yield return new WaitForSeconds(t1);
        GameController.instance.BossPanel.SetActive(true);
        yield return new WaitForSeconds(t2);
        GameController.instance.BossPanel.SetActive(false);
    }

    //Called internally and from gamecontroller
    public Vector3 GetCurrentRoadPosition()
    {
        return LevelsPassed * new Vector3(0, RoadYOffset, RoadZOffset);
    }

    //This is called when the player hits current levels' building.
    public void SetBuildingFlag()
    {
        if (Tutorial.activeInHierarchy)
        {
            Tutorial.GetComponent<TutorialScript>().Building.GetComponent<BuildingScript>().flag = true;
        }
        else
        {
            //Set the flag to true so that the game may begin counting down
            SpawnedBuildings[LevelsPassed].flag = true;
        }
    }

    //Randomises the squad groups on the map, the total squad count must be greater than the designated number
    private void SpawnSquadsObstacles(Vector3 roadPos, LevelScript currentLevel, int numObstacles, GameObject round)
    {
        var numAllocationSpots = NumberOfRoadDivides / 2;
        //----------------------------Obstacle Allocation--------------------------------------------------
        List<int> obstacleAlloc = new List<int>(); //This list is a unique list of obstacle indexes generated at random
        obstacleAlloc = Enumerable.Repeat(0, numAllocationSpots).ToList();

        for (int i = 0; i < numObstacles; i++)
        {
            var ran = Random.Range(0, obstacleAlloc.Count); //Select a random index and add 1 to it
            while (obstacleAlloc[ran] >= 1) //Select another location if its already full
            {
                ran = Random.Range(0, obstacleAlloc.Count);
            }
            obstacleAlloc[ran]++;
        }

        //----------------------------Squad Allocation--------------------------------------------------
        int[] a = { 0, 1, 2, 3 };
        List<int> squadAlloc = new List<int>();
        squadAlloc = Enumerable.Repeat(0, numAllocationSpots).ToList(); //Populate the baskets with 0

        currentLevel.NumberOfSquads += DefaultSquadCountAdjust;

        //Check Statement to prevent the code from going infinite in while loop
        if (currentLevel.NumberOfSquads > squadAlloc.Count * 3)
        {
            Debug.Log("Total number of squads for this level exceeds the amount of allocation spots - Defaulting it to maximum count");
            currentLevel.NumberOfSquads = squadAlloc.Count * 3;
        }

        for (int i = 0; i < currentLevel.NumberOfSquads; i++)
        {
            var ran = Random.Range(0, squadAlloc.Count); //Select a random index and add 1 to it
            while (squadAlloc[ran] >= 3) //Select another location if its already full
            {
                ran = Random.Range(0, squadAlloc.Count);
            }
            squadAlloc[ran]++;
        }

        //Once the squad counts are allocated, run a loop spawning squads and obstacles
        float zOffset = FirstObstacleDistance;
        float squadXpos = -999f;
        for (int i = 0; i < squadAlloc.Count; i++)
        {
            float obstacleXpos = -999f;
            
            if(obstacleAlloc[i] > 0)
            {
                var xPos = Random.Range(-ObstacleXAxisDiv, ObstacleXAxisDiv + 1) * ObstacleXDistance / ObstacleXAxisDiv;
                while(xPos == squadXpos)
                {
                    xPos = Random.Range(-ObstacleXAxisDiv, ObstacleXAxisDiv + 1) * ObstacleXDistance / ObstacleXAxisDiv;
                }
                obstacleXpos = xPos;
                var zPos = FirstObstacleDistance + PlacementDistance * 2 * i + PlacementDistance; //2i*d + d
                var pos = roadPos + new Vector3(xPos, ObstacleStartingYPos, zPos);
                var obj = Instantiate(Obstacle, pos, Quaternion.identity);
                currentLevel.Obstacles.Add(obj);
                obj.transform.SetParent(round.transform);
            }
            squadXpos = -999f;
            if (squadAlloc[i] > 0)
            {
                var squadObj = SquadPrefabs[squadAlloc[i] - 1]; //Squad prefabs must be organised in an incremental manner in the inspector
                var xPos = Random.Range(-ObstacleXAxisDiv, ObstacleXAxisDiv + 1) * ObstacleXDistance / ObstacleXAxisDiv;
                while (xPos == obstacleXpos)
                {
                    xPos = Random.Range(-ObstacleXAxisDiv, ObstacleXAxisDiv + 1) * ObstacleXDistance / ObstacleXAxisDiv;
                }
                squadXpos = xPos;
                var zPos = FirstObstacleDistance + PlacementDistance * 2 * i; //2i*d
                var obj = Instantiate(squadObj, roadPos + new Vector3(xPos, 0, zPos), Quaternion.Euler(0,180,0));
                currentLevel.Squads.Add(obj);
                obj.transform.SetParent(round.transform);
            }
        }
    }

    //We only need to show maximum of two rounds - this is called from the start of this class as well as gamecontroller class when camera transitioning is complete
    public void ShowRounds()
    {
        for(int i = 0; i < NumberOfRoundsPerLevel; i++)
        {
            if (i == LevelsPassed || i == LevelsPassed + 1)
            {
                Rounds[i].SetActive(true);
            }
            else
            {
                Rounds[i].SetActive(false);
            }
        }
    }

    public void DecrementSquadCount()
    {
        InstantiatedLevels[LevelsPassed].NumberOfSquads--;
        if(InstantiatedLevels[LevelsPassed].NumberOfSquads == 0)
        {
            if (FeverTimeActive)
            {
                Fever();
            }
        }
    }

    public void Fever()
    {
        StartCoroutine(FeverTime());
    }

    IEnumerator FeverTime()
    {
        GameController.instance.SetPlayerSpeed(GameController.instance.PlayerSpeed * FeverTimeSlowmoScale);        
        GameController.instance.PlayerScript.PeRunFeverActivate();
        GameController.instance.Fever.SetActive(true);
        GameController.instance.peFeverText.SetActive(true);
        GameController.instance.FeverScore.SetActive(true);
        GameController.instance.IncrementCurrentscore(10);
        yield return new WaitForSeconds(FeverTimeSlowmoTime);
        GameController.instance.PlayerScript.PeRunFeverActivateAfter();
        Time.timeScale = 1;
        GameController.instance.PlayerScript.SetPlayerSpeed(GameController.instance.PlayerFeverSpeed);
        GameController.instance.Fever.SetActive(false);
        GameController.instance.peFeverText.SetActive(false);
        GameController.instance.FeverScore.SetActive(false);
    }
}
