using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Touch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject Player;
    public float xLimit;

    private bool dragging;
    private Vector2 previousPos;
    private Vector2 firstTouchPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        previousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 5));        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    } 

    private void Update()
    {
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 5));
        var movementOffset = currentPos.x - previousPos.x;

        if (dragging)
        {
            Player.transform.position += new Vector3(movementOffset, 0, 0);
            if (Mathf.Abs(Player.transform.position.x) > xLimit)
            {
                Player.transform.position -= new Vector3(movementOffset, 0, 0);
            }
        }

        previousPos = currentPos;
    }


}
