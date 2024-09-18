using Microsoft.VisualBasic.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityVisualManager : MonoBehaviour
{
    public DeckVisualHandler deckLoc;

    public CardContainer HandLoc;
    public CardContainer SideCardLoc;

    public Transform PlayLoc;
    public Transform DiscardLoc;

    public TextSpinManager ScoreText;
    public TextSpinManager MoneyText;

    public TextMeshProUGUI ScoreTextFade;
    public TextMeshProUGUI MoneyTextFade;

    public Button playButton;

    public EntityAudioManager Eaudio;

    public virtual void Init()
    {
        HandLoc.Init(GridLayoutGroup.Constraint.FixedRowCount, 1, true);
        SideCardLoc.Init(GridLayoutGroup.Constraint.FixedColumnCount, 1, true);
    }

    public IEnumerator AddToHand(ICardVisuals card) 
    {
        CoroutineWaitForList list = new CoroutineWaitForList();

        if (!card.Visible && this is PlayerVisualManager)
        {
            StartCoroutine(list.CountCoroutine(card.Flip(true)));
        }
        card.transform.SetParent(transform.parent);
        StartCoroutine(list.CountCoroutine(HandLoc.AddNewCard(card)));
        deckLoc.OrganizeDeck();
        yield return list;

        
    }

    public void PutToHand(ICardVisuals card)
    {
        card.transform.SetParent(transform.parent);
        HandLoc.PutNewCard(card);
        deckLoc.OrganizeDeck();
    }

    public IEnumerator AddToSideCards(ICardVisuals card)
    {
        SideCardLoc.SetupSlots(card);
        yield return SideCardLoc.AddNewCard(card);
    }

    public void PutToSideCards(ICardVisuals card)
    {
        SideCardLoc.SetupSlots(card);
        SideCardLoc.PutNewCard(card);
    }

    public void RemoveFromHand(params ICardVisuals[] cards)
    {
        HandLoc.RemoveCard(cards);
        HandLoc.UpdateLayoutSpacing();
    }

    public void RemoveFromSideCards(params ICardVisuals[] cards)
    {
        SideCardLoc.RemoveCard(cards);
    }

    public IEnumerator MoveCardToPlay(ICardVisuals card)
    {
        Coroutine cor = StartCoroutine(card.MoveCard(PlayLoc.position));
        HandLoc.UpdateLayoutSpacing();
        yield return cor;
    }

    public void UpdateDeck()
    { 
        
    }

    public IEnumerator AddToDeck(params ICardVisuals[] cards)
    { 
        CoroutineWaitForList list = new CoroutineWaitForList();

        foreach (var card in cards)
        {
            Eaudio.RecycleCard(card.GetPos());
            StartCoroutine(list.CountCoroutine(card.MoveCard(deckLoc.transform)));
            if (card.Visible)
            {
                StartCoroutine(list.CountCoroutine(card.Flip(false)));
            }
            card.DisableCardForPlayer();
            card.SetDraggable(false);
            deckLoc.AddCardToDeck(card);
        }

        yield return list;
        deckLoc.OrganizeDeck();
    }

    internal IEnumerator UpdateScore(int score)
    {
        ScoreText.Value = score;

        yield return ScoreText.CountingCoroutine;
    }

    internal IEnumerator UpdateMoney(int money)
    {
        MoneyText.Value = money;

        yield return MoneyText.CountingCoroutine;
    }

    public void ResetTexts(int score, int money)
    {
        ScoreText.SetTextInstant(score);
        MoneyText.SetTextInstant(money);
    }
}
