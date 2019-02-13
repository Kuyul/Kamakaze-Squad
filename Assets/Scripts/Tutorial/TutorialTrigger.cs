using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public TutorialScript tutorial;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "player")
        {
            tutorial.DisplayTutorial();
        }
    }
}
