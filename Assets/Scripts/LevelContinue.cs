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

    //Called when GameOver or Game clear to reset the data from the previous level
    //Here we destroy existing buildings
    public void ResetRound()
    {
        Destroy(Building);
        levelIsContinued = false;
    }
}