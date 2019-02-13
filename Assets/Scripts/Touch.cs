using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Touch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //Declare public variables
    public float xLimit;
    public float xLimitNormal = 5.0f;
    public float xLimitMax;

    public float sensitivity = 10.0f;
    public float maxRotation = 30f;
    public float rotationSensitivity = 5.0f;

    //Declare private variables
    private GameObject Player;
    private bool dragging;
    private bool newScene=true;
    private Vector2 previousPos;
    private Vector2 releaseTouchPos;

    void Start()
    {
        Player = GameController.instance.Player;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        previousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, sensitivity));

        if (newScene)
        {
                GameController.instance.GameStarted = true;
                GameController.instance.SwipeToPlay();
                newScene = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    } 

    private void Update()
    {
        if (GameController.instance.GameStarted)
        {
            Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, sensitivity));
            var movementOffset = currentPos.x - previousPos.x;

            //Change player facing angle depending on where the player is going
            float rotateTo = 0;
            if (movementOffset > 0.01)
            {
                rotateTo = maxRotation;
            }
            else if (movementOffset < -0.01)
            {
                rotateTo = -maxRotation;
            }
            else
            {
                rotateTo = 0;
            }

            if (dragging)
            {
                var newPosX = Player.transform.position.x + movementOffset;
                if (Mathf.Abs(newPosX) <= xLimit)
                {
                    Player.transform.position += new Vector3(movementOffset, 0, 0);
                    Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, Quaternion.Euler(0, rotateTo, 0), rotationSensitivity * Time.deltaTime);
                }
            }
            else
            {
                Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, Quaternion.Euler(0, 0, 0), rotationSensitivity * Time.deltaTime);
            }

            previousPos = currentPos;
        }
    }

    public void IncreaseXLimit()
    {
        xLimit = xLimitMax;
    } 

    public void ResetXLimit()
    {
        xLimit = xLimitNormal;
    }

    //Only used in tutorial script
    public void SetDraggingToFalse()
    {
        dragging = false;
    }
}
