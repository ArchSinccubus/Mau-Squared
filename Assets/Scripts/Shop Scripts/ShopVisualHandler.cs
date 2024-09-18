using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopVisualHandler : MonoBehaviour
{
    public ShopCardMenuHandler HandCards;

    public ShopCardMenuHandler SideCards;

    public TextMeshProUGUI MoneyText;

    public Button RemoveButton, RefreshButton, BuyButton, BuySideCardSpaceButton;

    Coroutine movement1, movement2;

    public ScreenMoverHelper mover;

    bool HoverDeck, HoverSide;
    bool DeckShow, SideShow;
    bool DeckLock, SideLock;

    public TextMeshProUGUI RefreshCost, RemoveCost, SideCost;

    public GameObject PriceShower;
    public TextMeshProUGUI PriceShowerText;

    private void Update()
    {

        RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        HoverDeck = Array.Exists(hit, o => o.collider.name == "Deck") || DeckLock;
        HoverSide = Array.Exists(hit, o => o.collider.name == "Side") || SideLock;


        if (!GameManager.currRun.playerVis.DeckMoving || !GameManager.instance.currShop.CanAct)
        {
            movement1 = null;
        }
        
        if (!GameManager.currRun.playerVis.SideMoving || !GameManager.instance.currShop.CanAct)
        {
            movement2 = null;
        }

        if (GameManager.instance.currShop.CanAct)
        {

            if (HoverDeck && !DeckShow)
            {
                if (movement1 != null)
                    StopCoroutine(movement1);
                movement1 = StartCoroutine(GameManager.currRun.playerVis.MoveDeck(true));
                DeckShow = true;
            }
            else if (!HoverDeck && DeckShow)
            {
                if (movement1 != null)
                    StopCoroutine(movement1);
                movement1 = StartCoroutine(GameManager.currRun.playerVis.MoveDeck(false));
                DeckShow = false;
            }

            if (HoverSide && !SideShow)
            {
                if (movement2 != null)
                    StopCoroutine(movement2);
                movement2 = StartCoroutine(GameManager.currRun.playerVis.MoveSide(true));
                SideShow = true;
            }
            else if (!HoverSide && SideShow)
            {
                if (movement2 != null)
                    StopCoroutine(movement2);
                movement2 = StartCoroutine(GameManager.currRun.playerVis.MoveSide(false));
                SideShow = false;
            }
        }
    }

    public void init(HandCardDataHandler[] handCards, SideCardDataHandler[] sideCards, ChoiceDelegate select)
    {
        HandCards.initMenu(GridLayoutGroup.Constraint.Flexible , handCards, 4, select);
        HandCards.cardContainer.group.childAlignment = TextAnchor.UpperLeft;
        SideCards.initMenu(GridLayoutGroup.Constraint.FixedColumnCount, sideCards, 1, select);

        GameManager.currRun.playerVis.deckLoc.DeckClick.AddDelegate(OpenDeck);

        DeckLock = false;
        SideLock = false;
    }

    public void Deload()
    {
        GameManager.currRun.playerVis.deckLoc.DeckClick.RemoveDelegate(OpenDeck);
    }


    public IEnumerator BuySideCard(ICardVisuals vis)
    {
        vis.SetSelect(false);

        yield return GameManager.currRun.playerVis.MoveSide(true);

        GameManager.currRun.playerVis.SideCardLoc.SetupSlots(vis);

        yield return GameManager.currRun.playerVis.SideCardLoc.AddNewCard(vis); 
        SideCards.cardContainer.RemoveCard(vis);

        yield return GameManager.currRun.playerVis.MoveSide(false);

    }

    public IEnumerator BuyHandCard(ICardVisuals vis)
    {
        HandCards.cardContainer.RemoveCard(vis);

        vis.SetSelect(false);
        StartCoroutine(vis.Flip(false));

        yield return GameManager.currRun.playerVis.MoveDeck(true);

        GameManager.currRun.playerVis.deckLoc.AddCardToDeck(vis);

        
        yield return GameManager.currRun.playerVis.AddToDeck(vis);
        
    }

    public IEnumerator FinishBuyHandCard(ICardVisuals vis)
    {

        yield return GameManager.currRun.playerVis.MoveDeck(false);
    }

    public void SelectCard(ICardVisuals vis, int price, bool Side)
    {
        if (Side)
        {
            SideCards.SelectCard(vis, price);
        }
        else
        {
            HandCards.SelectCard(vis, price);
        }
    }

    public void DeselectCard(ICardVisuals vis, bool Side) 
    {
        if (Side)
        {
            SideCards.DeselectCard(vis);
        }
        else
        {
            HandCards.DeselectCard(vis);
        }
    }

    public void ChangeLockState(ICardVisuals card, bool side, bool state)
    {
        if (side)
        {
            SideCards.ChangeLockState(card, state);
        }
        else
        {
            HandCards.ChangeLockState(card, state);
        }
    }

    public void ResetShop()
    {
        HandCards.ResetMenu();
        SideCards.ResetMenu();
    }

    public void UpdateMoney(int amount, bool canRefresh, bool canRemove, bool CanSideCard)
    {
        MoneyText.text = amount.ToString();

        RefreshButton.interactable = canRefresh;
        RemoveButton.interactable = canRemove;
        BuySideCardSpaceButton.interactable = CanSideCard;

    }

    public void UpdateRefreshPrice(int price)
    {
        RefreshCost.text = price + "$";
    }

    public void UpdateRemovePrice(int price)
    {
        RemoveCost.text = price + "$";
    }

    public void UpdateSidePrice(int price)
    {
        SideCost.text = price + "$";
    }

    public void UpdateBuyButton(bool able, bool sell) 
    {
        BuyButton.interactable = able;

        BuyButton.GetComponentInChildren<TextMeshProUGUI>().text = sell ? "Sell" : "Buy";
    }

    public void ShowSellPrice(ICardVisuals card, int price)
    {
        PriceShower.SetActive(true);
        PriceShower.transform.position = card.GetPos() + Vector2.down;
        PriceShowerText.text = price.ToString();
    }

    public void HideSellPrice()
    {
        PriceShower.SetActive(false);
        PriceShower.transform.position =  Vector2.zero;
        PriceShowerText.text = "";
    }

    public void OpenDeck()
    {
        DeckLock = true;
    }

    public void CloseDeck()
    {
        DeckLock = false;
    }

    public void LockSide()
    {
        SideLock = true;
    }

    public void UnlockSide()
    {
        SideLock = false;
    }
}
