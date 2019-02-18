using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFever : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "player")
        {
            LevelControl.instance.Fever();
        }
    }
}
