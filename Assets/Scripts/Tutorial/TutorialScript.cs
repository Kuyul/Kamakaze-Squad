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
    public GameObject LevelPanel;
    public GameObject TutorialText;
    public float TutPopupDelay;

    //Declare private variables
    private int count = 0;
    private GameObject ActiveTutorial;
    private bool Waiting = false;
    private bool Stop=true;
    private float CurrentTime=0;

    private void Start()
    {
        LevelPanel.SetActive(false);
        TutorialText.SetActive(true);
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
                Touch.GetComponent<Touch>().dragging = true;
                Touch.SetActive(true);
                ActiveTutorial.SetActive(false);
                Waiting = false;
                Time.timeScale = 1;
            }
        }
        if (!Stop)
        {
            if (Time.realtimeSinceStartup > CurrentTime + TutPopupDelay)
            {
                Waiting = true;
                Stop = true;
            }
        }
    }

    //Called from tutorial triggers
    public void DisplayTutorial()
    {
        Touch.SetActive(false);
        Tutorials[count].SetActive(true);
        ActiveTutorial = Tutorials[count];
        count++;
        CurrentTime = Time.realtimeSinceStartup;
        Stop = false;
        Touch.GetComponent<Touch>().dragging = false;
        Time.timeScale = 0;
    }
}
