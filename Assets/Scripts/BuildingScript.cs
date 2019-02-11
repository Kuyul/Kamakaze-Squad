using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    public bool SpinningBuilding = false;
    public GameObject Bomb;
    public GameObject[] Cubes;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.angularVelocity = new Vector3(0, 45, 0);
        }
    }

    public void SetKinematicCubes(bool b)
    {
        for (int i = 0; i < Cubes.Length; i++)
        {
            Cubes[i].GetComponent<Rigidbody>().isKinematic = b;
        }
    }

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
