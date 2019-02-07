using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    public GameObject Bomb;
    public GameObject[] Cubes;

    //Called from the Levelcontrol class to destroy all building blocks
    public void DestroyCubes()
    {
        for(int i = 0; i < Cubes.Length; i++)
        {
            Destroy(Cubes[i]);
         //   Instantiate(GameController.instance.peBlock, Cubes[i].transform.position, Quaternion.identity);
        }
    }
}
