using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadScript : MonoBehaviour
{
    public GameObject[] EvenRoads;
    public GameObject[] OddRoads;

    public Material[] EvenMaterials;
    public Material[] OddMaterials;

    private void Start()
    {
        var lvl = LevelControl.instance.GetCurrentLevel();
        if (EvenMaterials.Length > 0)
        {
            var index = (lvl - 1) % EvenMaterials.Length;

            for (int i = 0; i < EvenRoads.Length; i++)
            {
                EvenRoads[i].GetComponent<MeshRenderer>().material = EvenMaterials[index];
                OddRoads[i].GetComponent<MeshRenderer>().material = OddMaterials[index];
            }
        }
    }
}
