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
    public void ResetLevel()
    {
        Destroy(Building);
        levelIsContinued = false;
        TriesLeft = 0;
    }
}