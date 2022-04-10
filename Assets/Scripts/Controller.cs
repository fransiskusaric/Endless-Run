using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private float touchX;
    private bool isJump;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isJump)
        {
            EventManager.TriggerEvent("jump");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchX = eventData.position.x;
        isJump = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (touchX < eventData.position.x)
        {
            // Swipe to right
            if (ControlPoint.main.characterModel.lane != CharacterModel.Lane.Right)
            {
                EventManager.TriggerEvent("right");
            }
        }
        else if (touchX > eventData.position.x)
        {
            // Swipe to left
            if (ControlPoint.main.characterModel.lane != CharacterModel.Lane.Left)
            {
                EventManager.TriggerEvent("left");
            }
        }
        else
        {
            isJump = true;
        }
    }
}
