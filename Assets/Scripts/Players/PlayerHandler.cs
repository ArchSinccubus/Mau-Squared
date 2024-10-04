using Assets.Scripts.Auxilary;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerHandler : EntityHandler
{
    [JsonIgnore]
    PlayerVisualManager Pvisuals => visuals as PlayerVisualManager;

    [JsonIgnore]
    bool CanPlay;

    [JsonIgnore]
    public override PlayerData playerData 
    {
        get {
            PlayerData temp = GameManager.currRun.baseData;

            foreach (IPlayerDecorator item in playerDecorators)
            {
                temp = item.Decorate(temp);
            }
            return temp;
        }
        
        set => GameManager.currRun.baseData = value; 
    }

    [JsonIgnore]
    public Button playButton;

    public PlayerHandler(DeckSO deck, PlayerVisualManager vis) : base(deck.DeckBase, true)
    { 
        visuals = vis;
    }

    [JsonConstructor]
    public PlayerHandler(bool cardPlayed, bool isPlayer, DeckHandler currDeck, List<HandCardDataHandler> hand, int sideCardDeckSize, List<SideCardDataHandler> sideCards, int score, int roundMoney, 
        bool activatedEffects, List<CardDecorateData> cardDecorators, List<IPlayerDecorator> playerDecorators, PlayerData baseData, bool madeSelection, PlayerData playerData) :
        base(cardPlayed, isPlayer, currDeck, hand, sideCardDeckSize, sideCards, score, roundMoney, activatedEffects, cardDecorators, playerDecorators,playerData )
    {
        currDeck.owner = this;

        foreach (var item in Hand)
        {
            item.owner = this;
        }

        foreach (var item in currDeck.DeckBase)
        {
            item.owner = this;
        }

        foreach (var item in sideCards)
        {
            item.owner = this;
        }

    }

    public override void InitNewRound(EntityVisualManager loc, EntityAudioManager aud)
    {
        base.InitNewRound(loc, aud);

        Pvisuals.ToggleTurnText(true);

        visuals.playButton.onClick.AddListener(MakeSelection);
    }

    public override void InitNewRound(EntityHandler orig, EntityVisualManager loc, EntityAudioManager aud)
    {
        base.InitNewRound(orig, loc, aud);

        Pvisuals.ToggleTurnText(true);

        visuals.playButton.onClick.AddListener(MakeSelection);

        foreach (SideCardDataHandler item in SideCards)
        {
            visuals.PutToSideCards(item.visuals);
        }
    }

    public void InitOutOfRound(EntityHandler orig, EntityVisualManager loc)
    {
        visuals = loc;

        Pvisuals.ToggleTurnText(false);

        visuals.Init();

        foreach (SideCardDataHandler item in SideCards)
        {
            item.InitForView(true, 0, false);
            visuals.PutToSideCards(item.visuals);
        }

        currDeck.LoadDeck(loc.deckLoc);

        CanAct = false;
    }

    public override IEnumerator StartRound()
    {
        Pvisuals.UpdateSideCardText(SideCards.Count, playerData.MaxSideCards);

        RoundMoney = playerData.PlayerMoney;

        yield return (visuals as PlayerVisualManager).MoveLayout(true);

        visuals.ResetTexts(0, RoundMoney);

        CoroutineWaitForList list = new CoroutineWaitForList();

        int DrawFinal = playerData.StartHandSize - GameManager.currRun.MauCardP;

        if (DrawFinal < 2)
        {
            DrawFinal = 2;
        }

        visuals.StartCoroutine(list.CountCoroutine(Draw(DrawFinal)));
        visuals.StartCoroutine(list.CountCoroutine(SetupSide()));

        yield return list;

        if (GameManager.currRun.MauPunishP)
        {
            yield return SmokeCards(GameManager.currRun.roundScene.getNPC(), Hand.ToArray());
        }

        GameManager.currRun.MauCardP = 0;
        GameManager.currRun.MauPunishP = false;
    }

    public override IEnumerator SetupSide()
    {
        foreach (SideCardDataHandler card in SideCards)
        {
            card.Used = false;
                        
            if (card.data.Clickable)
            {
                card.PlayerControl = true;
            }

            if (card is ISubscriber)
            {
                (card as ISubscriber).Subscribe(card);
            }
        }

        yield return new WaitForGameEndOfFrame();
    }

    public override IEnumerator EndRound()
    {
        CoroutineWaitForList list = new CoroutineWaitForList();

        foreach (var item in Hand)
        {
            if (!item.temp)
            {
                currDeck.AddCardToShuffle(item);
                visuals.StartCoroutine(list.CountCoroutine(visuals.AddToDeck(item.visuals)));
                visuals.RemoveFromHand(item.visuals);
            }
            else
            { 
                visuals.RemoveFromHand(item.visuals);
                item.ClearForRound();
            }
        }

        Hand.Clear();

        yield return list;

        visuals.playButton.onClick.RemoveAllListeners();

        foreach (var item in currDeck.DeckBase)
        {
            item.ClearForRound();
        }
        foreach (var item in SideCards)
        {
            item.ClearForRound();
        }

        Pvisuals.ToggleTurnText(false);
    }

    public override void UpdateHand()
    {
        base.UpdateHand();
        foreach (var item in Hand)
        {
            item.PlayerControl = true;
            item.visuals.SetDraggable(true);
            item.visuals.SetUsable(item.Playable);
        }
    }

    public override IEnumerator ExecuteTurn()
    {
        yield return base.ExecuteTurn();

        selectedCard = null;

        madeSelection = false;

        UpdateHand();
        GameManager.SaveGame();
        CanPlay = CanAnyCardBePlayed();
        CanAct = true;

        Pvisuals.SetButtonText(CanPlay);
        Pvisuals.SetNote(!CanPlay);

        //Debug.Log(JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore }));
        GameManager.EnableDrag();
        while (!madeSelection)
        {
            yield return new WaitForGameEndOfFrame();
        }
        GameManager.DisableDrag();

        Pvisuals.SetNote(false);
        yield return selectedCard;
    }

    public override IEnumerator TriggerCard(BaseCardDataHandler card)
    {
        HandCardDataHandler topCard = GameManager.Round.ReturnTopCardData();
        if (card is HandCardDataHandler)
        {
            HandCardDataHandler temp = (HandCardDataHandler)card;
            if ((temp.state == HandCardState.InPile && topCard != card) || temp.state == HandCardState.InDeck)
            {
                yield return temp.Peek();
                yield return RevealCard(temp);
            }


            if (temp.data.Transformer)
            {
                yield return HideCard(temp);
            }


            audio.TriggerCard(card.visuals.GetPos());

            yield return card.TriggerCardEffect();

        }
        else if (card is SideCardDataHandler)
        {
            audio.TriggerCard(card.visuals.GetPos());
            yield return card.TriggerCardEffect();
        }
    }

    public override IEnumerator TriggerCard(BaseCardDataHandler card, string text)
    {
        HandCardDataHandler topCard = GameManager.Round.ReturnTopCardData();
        if (card is HandCardDataHandler)
        {
            HandCardDataHandler temp = (HandCardDataHandler)card;
            if ((temp.state == HandCardState.InPile && topCard != card) || temp.state == HandCardState.InDeck)
            {
                yield return temp.Peek();
                yield return RevealCard(temp);
            }


            if (temp.data.Transformer)
            {
                yield return HideCard(temp);
            }

            if (text != "")
            {
                audio.TriggerCard(card.visuals.GetPos());

                yield return card.TriggerCardEffect(text);
            }
        }
    }

    public override IEnumerator FinishTriggerCard(HandCardDataHandler card)
    {
        HandCardDataHandler topCard = GameManager.Round.ReturnTopCardData();

        if (card.data.Transformer)
        {
            yield return RevealCard(card);
        }

        if (card.state == HandCardState.InDeck)
        {
            yield return HideCard(card);
        }
        yield return card.Return();
    }

    public override void PlaceCard(HandCardDataHandler card)
    {
        base.PlaceCard(card);

        GameManager.currRun.roundScene.CompareScores(card.ReturnCalcScore());
    }

    public bool CanAnyCardBePlayed()
    {
        foreach (var item in Hand)
        {
            if (item.Playable)
            {
                return true;
            }
        }

        return false;
    }

    public override IEnumerator Draw(int num)
    {
        yield return base.Draw(num);
    
        foreach (var item in Hand)
        {
            item.visuals.EnableCardForPlayer();
        }
    }

    public override IEnumerator MakeChoice(HandCardDataHandler[] cards, string text, int amount, CardMenuChoiceMode mode, SetChoiceType choiceType)
    {
        if (cards.Length > 0)
        {
            if (mode == CardMenuChoiceMode.Forced && cards.Length <= amount)
            {
                yield return cards;
            }
            else
            {
                CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GameManager.Round.MakeChoice(cards, text, amount, mode));

                yield return cd.coroutine;

                while (cd.result == null)
                {
                    yield return new WaitForGameEndOfFrame();

                }

                yield return new WaitForGameEndOfFrame();

                yield return cd.result;
            }
        }
        else
        {
            yield return new HandCardDataHandler[0];
        }

    }

    public override IEnumerator MakeHandChoice(string text, int amount, CardMenuChoiceMode mode, Predicate<HandCardDataHandler> query, HandChoiceType choiceType)
    {
        if (Hand.Count > 0)
        {
            if (mode == CardMenuChoiceMode.Forced && Hand.Count <= amount)
            {
                HandCardDataHandler[] cards = Hand.ToArray();

                yield return cards;
            }
            else
            {
                CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GameManager.Round.MakeHandChoice(this, text, amount, mode, query));

                yield return cd.coroutine;

                while (cd.result == null)
                {
                    yield return new WaitForGameEndOfFrame();

                }

                yield return cd.result;
            }
        }
        else
        {
            yield return new HandCardDataHandler[0];
        }
    }

    public override void PlayCard(HandCardDataHandler card)
    {
        card.visuals.DisableCardForPlayer();
        card.visuals.SetSelect(false);
        card.visuals.SetHighlightable(false);
        base.PlayCard(card);
    }

    public override void selectCard(HandCardDataHandler card)
    {
        if (CanAct)
        {
            if (selectedCard != null)
            {
                selectedCard.visuals.SetSelect(false);

            }

            selectedCard = card;
            card.visuals.SetSelect(true);
        }
    }

    public void MakeSelection()
    {
        if (CanAct)
        {
            if (CanPlay)
            {
                if (selectedCard != null)
                {
                    CanAct = false;
                    madeSelection = true;
                }
            }
            else
            {
                CanAct = false;
                madeSelection = true;
            }
        }
    }

    public bool CanAddSideCard()
    {
        return SideCards.Count < playerData.MaxSideCards;
    }

    public override void AddSideCard(SideCardDataHandler newCard)
    {
        base.AddSideCard(newCard);
        Pvisuals.UpdateSideCardText(SideCards.Count, playerData.MaxSideCards);
    }

    public override void RemoveSideCard(SideCardDataHandler card)
    {
        base.RemoveSideCard(card);
        Pvisuals.UpdateSideCardText(SideCards.Count, playerData.MaxSideCards);
    }

    public override IEnumerator AddMoney(int amount)
    {
        GameManager.currRun.baseData.PlayerMoney += amount;
        yield return base.AddMoney(amount);

        Debug.Log("Remember to delete the shop update here! This is for debug only!");
        if (GameManager.currRun.runState == GameState.InShop)
        {
            GameManager.instance.currShop.currMoney = GameManager.currRun.baseData.PlayerMoney;
        }
    }

    public override IEnumerator ReduceMoney(int amount)
    {
        GameManager.currRun.baseData.PlayerMoney -= amount;
        yield return base.ReduceMoney(amount);
    }

    public override IEnumerator SetMoney(int amount)
    {
        GameManager.currRun.baseData.PlayerMoney = amount;
        return base.SetMoney(amount);
    }

    public override void IncreaseSideCardDeck()
    {
        GameManager.currRun.IncreaseSideCardSize();
        Pvisuals.UpdateSideCardText(SideCards.Count, playerData.MaxSideCards);
    }

    public override IEnumerator SmokeCards(EntityHandler cause, params HandCardDataHandler[] cards)
    {
        yield return base.SmokeCards(cause, cards);

        GameManager.Round.TimesSmoked += cards.Length;
    }

    public override IEnumerator RecycleCards(params HandCardDataHandler[] cards)
    {
        yield return base.RecycleCards(cards);

        GameManager.Round.TimesRecycled += cards.Length;

    }

    public override IEnumerator CleanCards(params HandCardDataHandler[] cards)
    {
        yield return base.CleanCards(cards);

        GameManager.Round.TimesCleaned += cards.Length;
    }
}
