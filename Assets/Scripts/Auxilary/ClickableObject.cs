using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void OnClickEvent();


public class ClickableObject : MonoBehaviour, IPointerClickHandler
{
    OnClickEvent onClickEvent;

    public void SetDelegate(OnClickEvent eve)
    {
        onClickEvent = eve;
    }


    public void AddDelegate(OnClickEvent eve)
    {
        onClickEvent += eve;
    }

    public void RemoveDelegate(OnClickEvent eve)
    {
        onClickEvent -= eve;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickEvent.Invoke();
    }
}
