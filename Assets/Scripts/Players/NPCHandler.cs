using Assets.Scripts.Auxilary;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCHandler : EntityHandler
{
    [JsonConverter(typeof(AIJsonConverter))]
    public IAIBase AI;

    public NPCHandler(IAIBase AItype, List<HandCardSO> deck, List<SideCardSO> sideDeck, int Money) : base(deck, false)
    {
        AI = AItype;
        playerData = new PlayerData() { StartHandSize = GameManager.STARTING_HAND_BASE, MaxSideCards = GameManager.STARTING_SIDE_DECK_SIZE, OutOfOptionsDrawAmount = 1 };

        foreach (var item in sideDeck)
        {
            SideCards.Add(new SideCardDataHandler(item, this, false));
        }

        RoundMoney = Money;
    }

    [JsonConstructor]
    public NPCHandler(RandomAI AIType, bool cardPlayed, bool isPlayer, DeckHandler currDeck, List<HandCardDataHandler> hand, int sideCardDeckSize, List<SideCardDataHandler> sideCards, int score, int roundMoney,
    bool activatedEffects, List<CardDecorateData> cardDecorators, List<IPlayerDecorator> playerDecorators, PlayerData baseData, bool madeSelection, PlayerData playerData) :
    base(cardPlayed, isPlayer, currDeck, hand, sideCardDeckSize, sideCards, score, roundMoney, activatedEffects, cardDecorators, playerDecorators, playerData)
    {
        currDeck.owner = this;

        if (Hand.Safe().Any())
        {
            foreach (var item in Hand)
            {
                item.owner = this;
            }
        }

        foreach (var item in currDeck.DeckBase)
        {
            item.owner = this;
        }

        if (sideCards.Safe().Any())
        {
            foreach (var item in sideCards)
            {
                item.owner = this;
            }
        }
    }

    public override IEnumerator StartRound()
    {
        visuals.ResetTexts(0,RoundMoney);

        CoroutineWaitForList list = new CoroutineWaitForList();

        int DrawFinal = playerData.StartHandSize - GameManager.currRun.MauCardE;

        if (DrawFinal < 2)
        {
            DrawFinal = 2;
        }

        visuals.StartCoroutine(list.CountCoroutine(Draw(DrawFinal)));
        visuals.StartCoroutine(list.CountCoroutine(SetupSide()));

        yield return list;

        if (GameManager.currRun.MauPunishE)
        {
            yield return SmokeCards(GameManager.currRun.roundScene.getNPC(), Hand.ToArray());
        }

        GameManager.currRun.MauCardE = 0;
        GameManager.currRun.MauPunishE = false;
    }

    public override IEnumerator LoadRound()
    {
        yield return base.LoadRound();

        foreach (var item in Hand)
        {
            item.PlayerControl = false;
        }
    } 

    public override void InitNewRound(EntityVisualManager loc, EntityAudioManager aud)
    {
        base.InitNewRound(loc, aud);

        foreach (var item in SideCards)
        {
            item.data.OnPickup(item);
        }
    }

    public override void InitNewRound(EntityHandler orig, EntityVisualManager loc, EntityAudioManager aud)
    {
        base.InitNewRound(orig, loc, aud);

        foreach (var item in currDeck.DeckBase)
        {
            item.owner = this;
        }

        foreach (var item in Hand)
        {
            item.owner = this;
        }

        foreach (var item in SideCards)
        {
            item.owner = this;
        }
    }

    public override IEnumerator ExecuteTurn()
    {
        selectedCard = null;
        yield return base.ExecuteTurn();

        CardPlayed = false;
        UpdateHand();

        yield return AI.ExecuteTurn(this, Hand.Where(o => o.Playable).ToList());

        if (selectedCard != null)
        {
            yield return selectedCard;
        }
    }

    public override IEnumerator MakeChoice(HandCardDataHandler[] cards, string text, int amount, CardMenuChoiceMode mode, SetChoiceType choiceType)
    {
        if (cards.Length > 0)
        {
            if (mode == CardMenuChoiceMode.Forced && Hand.Count <= amount)
            {
                yield return cards;
            }
            else
            {
                HandCardDataHandler[] choices = AI.ChooseOption(cards,this, amount, mode, null, choiceType);
                yield return choices;
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
                HandCardDataHandler[] choices = AI.ChooseHandOption(Hand.ToArray(),this, amount, mode, query, choiceType);
                yield return choices;
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
        card.visuals.RevealCard();
        card.Visible = true;
        base.PlayCard(card);
    }

    public override IEnumerator TriggerCard(BaseCardDataHandler card)
    {
        HandCardDataHandler topCard = GameManager.Round.ReturnTopCardData();

        if (card is HandCardDataHandler)
        {
            HandCardDataHandler temp = (HandCardDataHandler)card;

            if ((temp.state == HandCardState.InPile && topCard != card) || temp.state == HandCardState.InDeck)
            {
                yield return card.visuals.Peek();
                temp.Peeking = true;
            }

            if (temp.state == HandCardState.InHand)
            {
                yield return RevealCard(temp);
            }

            if (temp.Visible)
            {
                audio.TriggerCard(card.visuals.GetPos());

                yield return card.TriggerCardEffect();
            }
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
                yield return card.visuals.Peek();
                temp.Peeking = true;
            }

            if (temp.state == HandCardState.InHand)
            {
                yield return RevealCard(temp);
            }

            if (text != "" && card.visuals.Visible)
            {
                audio.TriggerCard(card.visuals.GetPos());

                if (temp.Visible)
                {
                    yield return card.TriggerCardEffect(text);
                }
                else
                {
                    yield return card.TriggerCardEffect();
                }
            }
        }
        else if (card is SideCardDataHandler)
        {
            audio.TriggerCard(card.visuals.GetPos());

            yield return card.TriggerCardEffect(text);
        }
    }

    public override IEnumerator FinishTriggerCard(HandCardDataHandler card)
    {
        HandCardDataHandler topCard = GameManager.Round.ReturnTopCardData();

        if (card.state == HandCardState.InDeck || card.state == HandCardState.InHand)
        {
            yield return HideCard(card);
        }
        if ((card.state == HandCardState.InPile && topCard != card) || card.state == HandCardState.InDeck)
        {
            yield return card.visuals.Return();
        }
    }

    public override IEnumerator EndRound()
    {
        foreach (var item in currDeck.DeckBase)
        {
            item.ClearForRound();
        }
        foreach (var item in Hand)
        {
            visuals.RemoveFromHand(item.visuals);
            item.ClearForRound();
        }
        foreach (var item in SideCards)
        {
            visuals.RemoveFromSideCards(item.visuals);
            item.ClearForRound();
        }

        yield return new WaitForGameEndOfFrame();
    }
}
