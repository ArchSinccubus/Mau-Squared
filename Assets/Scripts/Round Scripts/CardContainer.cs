using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardContainer : MonoBehaviour
{
    public GridLayoutGroup group;
    public RectTransform rectTrans;

    public List<CardSlotHandler> slotHandlers;
    public Dictionary<ICardVisuals, CardSlotHandler> slots;

    public float xPadding, yPadding, edgePedding;

    [HideInInspector]
    public float xSpaceAmount, ySpaceAmount;


    public CardSlotHandler prefab;

    [HideInInspector]
    public int RowCount, ColumnCount;

    float TrueHeight, TrueWidth;

    bool AlwaysAdd;

    public void Init(GridLayoutGroup.Constraint constraints, int amount, bool AddOverride)
    {
        group.constraint = constraints;
        group.constraintCount = amount;

        slots = new Dictionary<ICardVisuals, CardSlotHandler>();

        TrueHeight = rectTrans.rect.height;
        TrueWidth = rectTrans.rect.width;
        AlwaysAdd = AddOverride;
    }

    public void SetupSlots(params ICardVisuals[] newCards)
    {
        bool newList = slotHandlers.Count == 0;

        for (int i = 0; i < newCards.Length; i++)
        {
            if (newList || AlwaysAdd)
            {
                CardSlotHandler handler = Instantiate(prefab, transform, true);
                slots.Add(newCards[i], handler);
                slotHandlers.Add(handler);

                handler.init(newCards[i], transform, this);
            }
            else
            {
                CardSlotHandler handler = slotHandlers[i];
                slots.Add(newCards[i], handler);

                handler.init(newCards[i], transform, this);
            }


            if (group.constraintCount == 1 && group.constraint != GridLayoutGroup.Constraint.Flexible)
            {
                float mult = 1;
                switch (group.constraint)
                {
                    case GridLayoutGroup.Constraint.FixedColumnCount:
                        if (TrueWidth < newCards[i].rect.rect.width)
                        {
                            mult = (TrueWidth - edgePedding) / newCards[i].rect.rect.width;
                        }
                        break;
                    case GridLayoutGroup.Constraint.FixedRowCount:
                        if (TrueHeight < newCards[i].rect.rect.height)
                        {
                            mult = (TrueHeight - edgePedding) / newCards[i].rect.rect.height;
                        }
                        break;
                    default:
                        break;
                }
                if (mult != 1)
                {
                    newCards[i].ResizeCard(mult);
                }
            }
            newCards[i].SetOrigSize();
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    public IEnumerator AddNewCard(ICardVisuals newCard)
    {
        GetColumnAndRow(group, group.transform.childCount, out ColumnCount, out RowCount);

        UpdateLayoutSpacing();

        yield return new WaitForGameEndOfFrame();

        yield return newCard.MoveCard(slots[newCard].transform.position);
    
        newCard.SetSlotLoc(slots[newCard].transform);

        slots[newCard].active = true;
    }

    public void PutNewCard(ICardVisuals newCard)
    {

        GetColumnAndRow(group, group.transform.childCount, out ColumnCount, out RowCount);

        UpdateLayoutSpacing();

        newCard.SetPos(slots[newCard].transform.position);

        newCard.SetSlotLoc(slots[newCard].transform);

        slots[newCard].active = true;
    }

    public void RemoveCard(params ICardVisuals[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {

            Vector3 currPos = cards[i].GetPos();

            CardSlotHandler slot = slots[cards[i]];

            slots.Remove(cards[i]);

            Destroy(slot.gameObject);

            cards[i].SetPos(currPos);
        }
        GetColumnAndRow(group, group.transform.childCount -1, out ColumnCount, out RowCount);
    }

    public void RemoveCards()
    {
        RemoveCard(slots.Keys.ToArray());
    }

    public void DestroyCards()
    {
        if (slots != null)
        {
            foreach (var item in slots)
            {
                PoolingManager.ReturnToPool(item.Key);

                Destroy(item.Value.gameObject);

            }
        }

        slotHandlers = new List<CardSlotHandler>();
        slots = new Dictionary<ICardVisuals, CardSlotHandler>();
        //slots.Clear();
    }

    public void RemoveAllCards()
    {
        foreach (var item in slots)
        {
            PoolingManager.ReturnToPool(item.Key);
            item.Value.deload();
        }
        slots = new Dictionary<ICardVisuals, CardSlotHandler>();

    }

    public void SetSpacing(float xSpace, float ySpace)
    {
        xSpaceAmount = xSpace + xPadding;
        ySpaceAmount = ySpace + yPadding;
        group.spacing = new Vector2(xSpaceAmount, ySpaceAmount);
    }

    private void Update()
    {
        //group.spacing = new Vector2(xSpaceAmount, ySpaceAmount);
    }

    public void UpdateLayoutSpacing()
    {
        float width = group.cellSize.x;
        float height = group.cellSize.y;

        int xDotCount = RowCount;
        int yDotCount = ColumnCount;

        float xSpacing = (rectTrans.rect.width - (xDotCount * width)) / xDotCount;
        float ySpacing = (rectTrans.rect.height - (yDotCount * height)) / yDotCount;

        SetSpacing(xSpacing, ySpacing);
    }

    public int getGridPosX(int x)
    {
        return x % RowCount;
    }

    public int getGridPosY(int y)
    {
        return y / RowCount;
    }

    void GetColumnAndRow(GridLayoutGroup glg, int ChildCount, out int column, out int row)
    {
        column = 0;
        row = 0;

        if (ChildCount <= 0)
            return;

        //Column and row are now 1
        column = 1;
        row = 1;

        //Get the first child GameObject of the GridLayoutGroup
        RectTransform firstChildObj = glg.transform.
            GetChild(0).GetComponent<RectTransform>();

        Vector2 firstChildPos = firstChildObj.anchoredPosition;
        bool stopCountingRow = false;

        //Loop through the rest of the child object
        for (int i = 1; i < ChildCount; i++)
        {
            //Get the next child
            RectTransform currentChildObj = glg.transform.
           GetChild(i).GetComponent<RectTransform>();

            Vector2 currentChildPos = currentChildObj.anchoredPosition;

            //if first child.x == otherchild.x, it is a column, ele it's a row
            if (firstChildPos.x == currentChildPos.x)
            {
                column++;
                //Stop couting row once we find column
                stopCountingRow = true;
            }
            else
            {
                if (!stopCountingRow)
                    row++;
            }
        }
    }
}
