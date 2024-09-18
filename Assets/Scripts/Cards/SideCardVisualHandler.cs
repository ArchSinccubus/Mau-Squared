
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SideCardVisualHandler : CardVisualHandler
{
    public EventDelegate OnFinishDragEvent { get; set; }
    public EventDelegate OnStartDragEvent { get; set; }

    public void initSide(CardSOBase data, bool highlight, bool side, EventDelegate deg, int CanvasOverride)
    {
        init(data, highlight, side, CanvasOverride);

        OnFinishDragEvent = deg;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (Draggable)
        {
            dragged = true;
            if (OnStartDragEvent != null)
            {
                OnStartDragEvent.Invoke();
            }
            cardCanvas.overrideSorting = true;
            //SlotLoc = transform.parent;
            //transform.SetParent(GameManager.instance.canvas.transform);
            transform.SetAsLastSibling();
            StopAllCoroutines();
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (Draggable)
        {
            StartCoroutine(MoveCard(SlotLoc.position));
            GameManager.DisableDrag();
            dragged = false;

            //SlotLoc = null;
        }
    }

    public override IEnumerator MoveCard(Transform newParent)
    {
        yield return base.MoveCard(newParent);
        OnFinishDragEvent.Invoke();
    }

    public override void HideCard()
    {
        texture.color = Color.white;

        ShowBack = true;
        mat.SetInt("_Revealed", 0);

        texture.material = mat;

        Border.gameObject.SetActive(false);
    }

    public override void RevealCard()
    {
        mat.SetInt("_Revealed", 1);

        ShowBack = false;
        texture.material = mat;

        Border.gameObject.SetActive(true);
    }

    public override void UpdateCard(string newText)
    {
       
    }
}
