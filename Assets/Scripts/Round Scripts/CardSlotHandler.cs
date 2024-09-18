using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotHandler : MonoBehaviour
{
    ICardVisuals card;
    CardContainer container;
    public bool active;

    int R, C;
    int newSiblingIndex;
    int currSiblingIndex;

    private void Awake()
    {
        active = false;
    }

    public virtual void init(ICardVisuals newCard, Transform T, CardContainer parent)
    {
        transform.SetParent(T);
        transform.position = Vector3.zero;
        transform.SetAsLastSibling();

        transform.localScale = Vector3.one;

        card = newCard;
        container = parent;
        currSiblingIndex = parent.slots.Count;
        //Debug.Log(currSiblingIndex);
    }

    public virtual void deload()
    {
        card = null;
    }

    private void Update()
    {
        if (active)
        {
            if (card != null)
            {
                if (card.Dragged)
                {
                    Vector2 childPos = card.GetPos();

                    newSiblingIndex = transform.parent.childCount;

                    currSiblingIndex = transform.GetSiblingIndex();
                    //Debug.Log(currSiblingIndex);

                    if (currSiblingIndex + 1 < transform.parent.childCount)
                    {

                        if (childPos.x > transform.parent.GetChild(currSiblingIndex + 1).position.x && container.getGridPosY(currSiblingIndex) == container.getGridPosY(currSiblingIndex + 1))
                        {
                            currSiblingIndex++;
                        }
                    }

                    if (currSiblingIndex - 1 >= 0)
                    {

                        if (childPos.x < transform.parent.GetChild(currSiblingIndex - 1).position.x && container.getGridPosY(currSiblingIndex) == container.getGridPosY(currSiblingIndex - 1))
                        {
                            currSiblingIndex--;
                        }
                    }

                    if (currSiblingIndex + container.RowCount < transform.parent.childCount)
                    {

                        if (childPos.y < transform.parent.GetChild(currSiblingIndex + container.RowCount).position.y && container.getGridPosX(currSiblingIndex) == container.getGridPosX(currSiblingIndex + container.RowCount))
                        {
                            int temp = currSiblingIndex;
                            Transform sib = transform.parent.GetChild(currSiblingIndex + container.RowCount);
                            currSiblingIndex += container.RowCount;
                            sib.SetSiblingIndex(temp);
                        }
                    }

                    if (currSiblingIndex - container.RowCount >= 0)
                    {

                        if (childPos.y > transform.parent.GetChild(currSiblingIndex - container.RowCount).position.y && container.getGridPosX(currSiblingIndex) == container.getGridPosX(currSiblingIndex - container.RowCount))
                        {
                            int temp = currSiblingIndex;
                            Transform sib = transform.parent.GetChild(currSiblingIndex - container.RowCount);
                            currSiblingIndex -= container.RowCount;
                            sib.SetSiblingIndex(temp);
                        }
                    }
                    transform.SetSiblingIndex(currSiblingIndex);
                }
                //Debug.Log(transform.position.x);
            }

                          
            //if (card == null)
            //{
            //    //Destroy(gameObject);
            //}
        }
    }
}
