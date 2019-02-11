using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelContinue : MonoBehaviour
{
    public static LevelContinue instance;

    //Declare public variables
    [HideInInspector]
    public bool levelIsContinued = false;
    [HideInInspector]
    public GameObject Building;
    [HideInInspector]
    public int TriesLeft = 0;
    //[HideInInspector]
    public int LevelsPassed = 0;

    public int EnemysThisLevel;

    //This gameobject will persist between scene reloads as we need to keep track of tries counts and building states.
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        EnemysThisLevel = LevelControl.instance.InstantiatedLevels[LevelsPassed].Enemy;
    }

    //Called when GameOver or Game clear to reset the data from the previous level
    //Here we destroy existing buildings
    public void ResetRound()
    {
        Destroy(Building);
        levelIsContinued = false;
        TriesLeft = 0;
    }

    public void ResetLevel()
    {
        LevelsPassed = 0;
    }

    public void IncrementEnemyCount()
    {
        EnemysThisLevel--;
        if (EnemysThisLevel == 0)
        {
            StartCoroutine(Delay(2f));
        }
    }

    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
        LevelControl.instance.LevelClear();
        EnemysThisLevel = LevelControl.instance.InstantiatedLevels[LevelsPassed].Enemy;
    }
}