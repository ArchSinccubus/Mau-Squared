using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardMenuHandler : CardMenuHandler
{
    public void initMenu(GridLayoutGroup.Constraint constraint, BaseCardDataHandler[] cards, int row, ChoiceDelegate pick)
    {
        cardContainer.Init(constraint, row, false);

        options = new List<BaseCardDataHandler>();

        foreach (var item in cards)
        {

            if (item.visuals == null)
            {
                item.InitForChoice(true, pick, 0);
            }

            options.Add(item);

            item.visuals.RevealCard();
            item.visuals.EnableCardForPlayer();
            item.visuals.SetDraggable(false);           
        }

        cardContainer.SetupSlots(GameManager.getCardVisuals(cards));

        foreach (var item in cards)
        {
            cardContainer.PutNewCard(item.visuals);

            item.visuals.SetPos(GameManager.GetTopOfScreen(item.visuals.transform.position));

            (cardContainer.slots[item.visuals] as ShopCardSlotHandler).RevealPriceTag(item.price);
        }

    }

    public void SelectCard(ICardVisuals card, int price)
    {
        card.SetSelect(true);
        StartCoroutine(card.ResizePop(1.25f));
        cardContainer.slots[card].transform.localScale = Vector3.one * 1.25f;
        (cardContainer as ShopCardContainer).OnCardSelect(price, card);
    }

    public void DeselectCard(ICardVisuals card) 
    {
        card.SetSelect(false);
        card.ResetSizeCard();
        cardContainer.slots[card].transform.localScale = Vector3.one;
        (cardContainer as ShopCardContainer).OnCardDeselect(card);
    }

    public void ChangeLockState(ICardVisuals card, bool state)
    {
        (cardContainer.slots[card] as ShopCardSlotHandler).ChangeLockState(state);    
    }

    public void ResetMenu()
    {

        cardContainer.DestroyCards();

        options.Clear();
    }
}
