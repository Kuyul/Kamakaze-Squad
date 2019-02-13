using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
    //Declare public variables
    public float MinSquadDistance = 15.0f;
    public GameObject Building;
    public int NumberOfTries = 1;
    public int NumberOfSquads = 7;
    public int NumberOfObstacles = 5;
    [HideInInspector]
    public List<GameObject> Obstacles = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> Squads = new List<GameObject>();

    public void ResetLevel()
    {
        //Destroy obstacles
        for (int i = 0; i < Obstacles.Count; i++)
        {
            Destroy(Obstacles[i]);
        }
        Obstacles.Clear();

        //Destroy all spawned squads
        for(int i = 0; i < Squads.Count; i++)
        {
            Destroy(Squads[i]);
        }
        Squads.Clear();
    }
}
