using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    public GameObject Bomb;
    public GameObject[] Cubes;

    public void DestroyCubes()
    {
        for(int i = 0; i < Cubes.Length; i++)
        {
            Destroy(Cubes[i]);
        }
    }
}
