using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    public GameObject[] Enemies;
    [HideInInspector]
    public bool flag;

    private void Update()
    {
        //Flag is set when the player collides with the blocks of this building - its set from levelcontroller
        if (flag)
        {
            var stillActive = true;
            for(int i = 0; i < Enemies.Length; i++)
            {
                stillActive = Enemies[i].activeInHierarchy;
                //If anything is still active return.
                if (stillActive)
                {
                    break;
                }
            }

            //If it remains false here, this means none of them are active and therefore game clear!
            if (!stillActive)
            {
                LevelControl.instance.LevelClear();
                flag = false;
            }
            else
            {
                for (int i = 0; i < Enemies.Length; i++)
                {
                    var vel = Enemies[i].GetComponent<Rigidbody>().velocity.magnitude;
                    //If any of these aren't moving, check with level controller whether to end the game or continue the round
                    if (vel < 0.1f)
                    {
                        LevelControl.instance.ContinueLevel();
                        flag = false;
                        break; //We only need to run this once
                    }
                }
            }
        }
    }
}
