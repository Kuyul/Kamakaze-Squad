using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadScript : MonoBehaviour
{
    public GameObject[] EvenRoads;
    public GameObject[] OddRoads;

    private void Start()
    {
        var lvl = LevelControl.instance.GetCurrentLevel();
        if (LevelControl.instance.EvenMaterials.Length > 0)
        {
            var index = (lvl - 1) % LevelControl.instance.EvenMaterials.Length;

            for (int i = 0; i < EvenRoads.Length; i++)
            {
                EvenRoads[i].GetComponent<MeshRenderer>().material = LevelControl.instance.EvenMaterials[index];
            }

            for(int i = 0; i < OddRoads.Length; i++)
            {
                OddRoads[i].GetComponent<MeshRenderer>().material = LevelControl.instance.OddMaterials[index];
            }
        }
    }
}
