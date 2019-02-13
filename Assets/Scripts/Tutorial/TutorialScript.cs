using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    //Declare public variables
    public GameObject[] Tutorials;
    public GameObject TutorialRoad;
    public GameObject Touch;
    public GameObject Building;

    //Declare private variables
    private int count = 0;
    private GameObject ActiveTutorial;
    private bool Waiting = false;

    private void Start()
    {
        //Initially set touch to inactive to prevent movement
        Touch.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(count == 0)
            {
                GameController.instance.SwipeToPlay();
            }

            if (Waiting)
            {
                ActiveTutorial.SetActive(false);
                Waiting = false;
                Time.timeScale = 1;
            }
        }
    }

    //Called from tutorial triggers
    public void DisplayTutorial()
    {
        //touch will be reactivated on first obstacle encounter
        if(count == 0)
        {
            Touch.SetActive(true);
        }

        Debug.Log("Tutorial" + count + " Activated");
        Tutorials[count].SetActive(true);
        ActiveTutorial = Tutorials[count];
        Waiting = true;
        Time.timeScale = 0;
        count++;

        //Stop dragging
        Touch.GetComponent<Touch>().SetDraggingToFalse();
    }
}
