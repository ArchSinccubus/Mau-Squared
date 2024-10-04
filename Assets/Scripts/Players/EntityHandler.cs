using Assets.Scripts.Auxilary;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

//Manages the active human player during a round. Nothing here is permanent and all permanent carriable effects go to RunManager;
public class EntityHandler
{
    [JsonIgnore]
    public bool CardPlayed;

    public bool IsPlayer;

    [JsonIgnore]
    public EntityVisualManager visuals;

    [JsonIgnore]
    public EntityAudioManager audio;

    public DeckHandler currDeck;

    [JsonProperty]
    public List<HandCardDataHandler> Hand;

    [JsonProperty]
    public int SideCardDeckSize;
    [JsonProperty]
    public List<SideCardDataHandler> SideCards;

    [JsonProperty]
    private int Score;

    public int RoundMoney;

    [JsonIgnore]
    private bool canAct;

    public bool ActivatedEffects;

    [JsonIgnore]
    public List<CardDecorateData> cardDecorators;

    [JsonIgnore]
    public List<IPlayerDecorator> playerDecorators;

    private PlayerData baseData;

    [JsonIgnore]
    public HandCardDataHandler selectedCard;
    public bool madeSelection;

    [JsonIgnore]
    public bool CanAct { get => canAct && !GameManager.Pause; set => canAct = value; }

    public virtual PlayerData playerData {
        get
        {
            PlayerData temp = baseData;

            foreach (IPlayerDecorator item in playerDecorators)
            {
                temp = item.Decorate(temp);
            }

            return temp;
        }

        set {
            baseData.StartHandSize = value.StartHandSize;
            baseData.MaxSideCards = value.MaxSideCards;
            baseData.OutOfOptionsDrawAmount = value.OutOfOptionsDrawAmount;
            baseData.PlayerMoney = value.PlayerMoney;
            baseData.CardsPlayed = value.CardsPlayed;
            baseData.CardsRecycled = value.CardsRecycled;
            baseData.TotalScore = value.TotalScore;
        }
    }

    

    public EntityHandler(List<HandCardSO> deck, bool player)
    {
        currDeck = new DeckHandler(this, deck);
        IsPlayer = player;
        cardDecorators = new List<CardDecorateData>();
        playerDecorators = new List<IPlayerDecorator>();
        SideCards = new List<SideCardDataHandler>();
    }

    public EntityHandler(EntityHandler orig)
    {
        currDeck = new DeckHandler(this, orig.currDeck.DeckBase);

        IsPlayer = orig.IsPlayer;
        cardDecorators = orig.cardDecorators;
        playerDecorators = orig.playerDecorators;

        SideCards = new List<SideCardDataHandler>();
        foreach (var item in orig.SideCards)
        {
            SideCards.Add(new SideCardDataHandler(item));
            SideCards[SideCards.Count - 1].owner = this;
        }

        foreach (var item in Hand)
        {
            item.owner = this;
        }
    }

    [JsonConstructor]
    public EntityHandler(bool cardPlayed, bool isPlayer, DeckHandler currDeck, List<HandCardDataHandler> hand, int sideCardDeckSize, List<SideCardDataHandler> sideCards, int score, int roundMoney, bool activatedEffects, 
        List<CardDecorateData> temporaryDecorators, List<IPlayerDecorator> temporaryPlayerDecorators, PlayerData baseData)
    {
        CardPlayed = cardPlayed;
        IsPlayer = isPlayer;
        this.currDeck = currDeck;
        Hand = hand;
        SideCardDeckSize = sideCardDeckSize;
        SideCards = sideCards;
        Score = score;
        RoundMoney = roundMoney;
        ActivatedEffects = activatedEffects;
        this.cardDecorators = new List<CardDecorateData>();
        this.playerDecorators = new List<IPlayerDecorator>();
        this.baseData = baseData;

        foreach (var item in this.currDeck.DeckBase)
        {
            item.owner = this;
            //item.data.OnPickup(item);
        }

        if (Hand.Safe().Any())
        {
            foreach (var item in Hand)
            {
                item.owner = this;
                //item.data.OnPickup(item);
            }
        }

        if (sideCards.Safe().Any())
        {
            foreach (var item in sideCards)
            {
                item.owner = this;
            }
        }
    }

    public virtual void InitNewRound(EntityVisualManager loc, EntityAudioManager aud)
    {
        visuals = loc;
        audio = aud;

        Hand = new List<HandCardDataHandler>();

        visuals.Init();

        foreach (SideCardDataHandler item in SideCards)
        {
            item.InitForRound(IsPlayer);
        }

        currDeck.InitForRound(loc.deckLoc);

        currDeck.Shuffle();
        CanAct = false;

        Score = 0;
        ActivatedEffects = false;
    }

    public virtual void InitNewRound(EntityHandler orig, EntityVisualManager loc, EntityAudioManager aud)
    {
        visuals = loc;
        audio = aud;

        visuals.Init();

        foreach (var item in Hand)
        {
            item.InitForRound(orig.IsPlayer);
        }

        foreach (SideCardDataHandler item in SideCards)
        {
            item.InitForRound(IsPlayer);
        }

        currDeck.InitForRound(loc.deckLoc);

        CanAct = false;

        Score = orig.Score;
        ActivatedEffects = orig.ActivatedEffects;
    }

    public virtual IEnumerator StartRound()
    {
        visuals.ResetTexts(0, RoundMoney);

        CoroutineWaitForList list = new CoroutineWaitForList();

        visuals.StartCoroutine(list.CountCoroutine(Draw(playerData.StartHandSize)));
        visuals.StartCoroutine(list.CountCoroutine(SetupSide()));

        yield return list;
    }

    public virtual IEnumerator LoadRound()
    {
        visuals.ResetTexts(Score, RoundMoney);

        CoroutineWaitForList list = new CoroutineWaitForList();

        visuals.HandLoc.SetupSlots(GameManager.getCardVisuals(Hand.ToArray()));

        foreach (var item in Hand)
        {
            visuals.PutToHand(item.visuals);
        }
        visuals.StartCoroutine(list.CountCoroutine(SetupSide()));

        yield return list;
    }

    public virtual void UpdateHand()
    {
        CheckPlayable();
        foreach (var item in Hand)
        {
            item.state = HandCardState.InHand;
        }
    }

    public virtual IEnumerator SetupSide()
    {
        CoroutineWaitForList list = new CoroutineWaitForList();

        foreach (SideCardDataHandler card in SideCards)
        {
            visuals.StartCoroutine(list.CountCoroutine(visuals.AddToSideCards(card.visuals)));
            if (card is ISubscriber)
            {
                (card as ISubscriber).Subscribe(card);
            }
        }

        yield return list;
    }

    public virtual IEnumerator Draw(int num)
    {
        List<HandCardDataHandler> cardsToAdd = new List<HandCardDataHandler>();

        if (currDeck.CanDrawXCards(num))
        {
            cardsToAdd = currDeck.Draw(num);
        }
        else
        {
            int diff = num - currDeck.DeckBase.Count;

            cardsToAdd = currDeck.Draw(currDeck.DeckBase.Count);

            for (int i = 0; i < diff; i++)
            {

                cardsToAdd.Add(GameManager.ReturnBasicCard(this));
            }
        }

        Hand.AddRange(cardsToAdd);

        CoroutineWaitForList list = new CoroutineWaitForList();
        visuals.HandLoc.SetupSlots(getCardVisuals(cardsToAdd));

        yield return new WaitForGameEndOfFrame();

        foreach (var card in cardsToAdd)
        {
            visuals.StartCoroutine(list.CountCoroutine(visuals.AddToHand(card.visuals)));
            audio.DrawCard(visuals.deckLoc.transform.position);
            yield return new WaitForGameSeconds(0.2f);
            currDeck.visuals.OrganizeDeck();
        }

        yield return list;
        UpdateHand();

        foreach (var item in cardsToAdd)
        {
            yield return ObserverManagerSystem.NotifyDrawnCard(item);
        }

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnDraw, this, cardsToAdd);


        yield return cardsToAdd;
    }

    public virtual IEnumerator ExecuteTurn()
    {
        foreach (var item in SideCards)
        {
            item.TempBool = false;
        }
        yield return new WaitForGameEndOfFrame();
    }

    public int GetScore()
    {
        return Score;
    }

    public virtual void PlayCard(HandCardDataHandler card)
    {
        Hand.Remove(card);
        card.state = HandCardState.InPlay;
    }

    public virtual void PlaceCard(HandCardDataHandler card)
    {
        GameManager.Round.AddCardToPile(card);
        card.state = HandCardState.InPile;
    }

    public virtual IEnumerator RecycleCards(params HandCardDataHandler[] cards)
    {
        DiscardEventArgs args = new DiscardEventArgs() { cards = cards, owner = this };

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnRecycle, this, args);

        Hand.RemoveAll(o => cards.Contains(o));

        ICardVisuals[] vis = getCardVisuals(cards.ToList());


        foreach (var item in cards)
        {
            item.visuals.RevealCard();
            item.state = HandCardState.InDeck;
            currDeck.AddCardToShuffle(item);

        }

        yield return visuals.AddToDeck(vis);
        currDeck.visuals.OrganizeDeck();
        visuals.RemoveFromHand(vis);

        yield return new WaitForGameSeconds(0.5f);

        yield return Draw(cards.Length);
    }

    public ICardVisuals[] getCardVisuals(List<HandCardDataHandler> cards)
    {
        ICardVisuals[] vis = new ICardVisuals[cards.Count];

        for (int i = 0; i < cards.Count; i++)
        {
            vis[i] = cards[i].visuals;
        }

        return vis;
    }

    public virtual IEnumerator AddScore(int amount)
    {
        Score += amount;

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnScoreChanged, this, Score);

        yield return visuals.UpdateScore(Score);
    }

    public virtual IEnumerator MultScore(float mult)
    {
        Score = Mathf.FloorToInt(Score * mult);

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnScoreChanged, this, Score);

        yield return visuals.UpdateScore(Score);
    }

    public virtual IEnumerator MakeChoice(HandCardDataHandler[] cards, string text, int amount, CardMenuChoiceMode mode, SetChoiceType choiceType)
    {
        yield return cards[0];
    }

    public virtual IEnumerator MakeHandChoice(string text, int amount, CardMenuChoiceMode mode, Predicate<HandCardDataHandler> query, HandChoiceType choiceType)
    {
        yield return Hand[0];
    }

    public virtual void selectCard(HandCardDataHandler card)
    {

    }

    public void CheckPlayable()
    {
        HandCardDataHandler TopCard = GameManager.Round.ReturnTopCardData();

        foreach (HandCardDataHandler item in Hand)
        {
            item.Playable = item.data.Playable(item, TopCard);

            if (TopCard != null)
            {
                if (TopCard.data.LockValue)
                {
                    item.Playable = item.returnModifiedData().cardValues.Contains(TopCard.data.cardValues[0]);
                }
            }            
        }
    }

    public HandCardData modifyCard(HandCardData card)
    {
        foreach (var item in cardDecorators)
        {
            card = item.decorator.Decorate(item.Caller, card);
        }

        return card;
    }
    
    /// <summary>
    /// To trigger a card visually
    /// </summary>
    /// <param name="card">Card to trigger</param>
    /// <param name="hide">Should the card be hidden for color changing effects? Also use Reveal Card later if yes!</param>
    /// <returns></returns>
    public virtual IEnumerator TriggerCard(BaseCardDataHandler card)
    {
        yield return new WaitForGameEndOfFrame();
    }


    public virtual IEnumerator TriggerCard(BaseCardDataHandler card, string text)
    {
        yield return new WaitForGameEndOfFrame();
    }

    public virtual IEnumerator FinishTriggerCard(HandCardDataHandler card)
    {
        yield return new WaitForGameEndOfFrame();
    }

    public virtual IEnumerator RevealCard(HandCardDataHandler card)
    {
        if (!card.Visible)
        {
            yield return card.visuals.Flip(true);
            card.Visible = true;
        }

        yield return new WaitForGameEndOfFrame();
    }

    public virtual IEnumerator HideCard(HandCardDataHandler card)
    {
        if (card.Visible)
        {
            yield return card.visuals.Flip(false);
            card.Visible = false;
        }
    }

    public virtual IEnumerator AddMoney(int amount)
    {
        RoundMoney += amount;

        yield return visuals.UpdateMoney(RoundMoney);

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnMoneyChanged, this, amount);
    }

    public virtual IEnumerator ReduceMoney(int amount)
    {
        RoundMoney -= amount;

        yield return visuals.UpdateMoney(RoundMoney);

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnMoneyChanged, this, -amount);
    }

    public virtual IEnumerator SetMoney(int amount)
    {
        int diff = RoundMoney - amount;
        RoundMoney = amount;

        yield return visuals.UpdateMoney(RoundMoney);

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnMoneyChanged, this, diff);
    }

    public virtual IEnumerator SmokeCards(EntityHandler cause, params HandCardDataHandler[] cards)
    {
        List<HandCardDataHandler> smokedCards = cards.ToList();
        CoroutineWaitForList list = new CoroutineWaitForList();

        foreach (HandCardDataHandler item in cards)
        {
            if (!item.Smoked)
            {
                audio.SmokeCard(item.visuals.GetPos());
                item.Smoked = true;
                GameManager.Round.StartCoroutine(list.CountCoroutine(item.visuals.Shake()));
                yield return new WaitForGameSeconds(0.1f);
            }
            else
            {
                smokedCards.Remove(item);
            }
        }

        yield return list;

        SmokeEventData data = new SmokeEventData() { cards = smokedCards, cause = cause };

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnCardSmoked, this, data);
    }

    public virtual IEnumerator CleanCards(params HandCardDataHandler[] cards)
    {
        List<HandCardDataHandler> cleanedCards = cards.ToList();
        CoroutineWaitForList list = new CoroutineWaitForList();

        foreach (HandCardDataHandler item in cards)
        {
            if (item.Smoked)
            {
                audio.CleanCard(item.visuals.GetPos());
                item.Smoked = false;
                GameManager.Round.StartCoroutine(list.CountCoroutine(item.visuals.Wiggle()));
                yield return new WaitForGameSeconds(0.1f);
            }
            else
            {
                cleanedCards.Remove(item);
            }
        }

        yield return ObserverManagerSystem.NotifyEvents(DictionaryTypes.OnCardCleared, this, cleanedCards);
    }

    public virtual IEnumerator EndRound()
    {
        foreach (var item in currDeck.DeckBase)
        {
            item.ClearForRound();
        }
        foreach (var item in Hand)
        {
            item.ClearForRound();
        }
        foreach (var item in SideCards)
        {
            item.ClearForRound();
        }

        yield return new WaitForGameEndOfFrame();
    }

    public virtual IEnumerator AddHandCard(HandCardDataHandler newCard)
    {
        Hand.Add(newCard);
        newCard.state = HandCardState.InHand;
        visuals.HandLoc.SetupSlots(newCard.visuals);

        yield return visuals.AddToHand(newCard.visuals);
        UpdateHand();
        newCard.data.OnPickup(newCard);
    }

    public virtual void AddSideCard(SideCardDataHandler newCard)
    {
        newCard.owner = this;
        SideCards.Add(newCard);
        newCard.data.OnPickup(newCard);
    }

    public virtual void RemoveSideCard(SideCardDataHandler card)
    {
        visuals.StartCoroutine(card.visuals.Vanish());
        SideCards.Remove(card);
        visuals.RemoveFromSideCards(card.visuals); 
        PoolingManager.ReturnToPool(card.visuals);
    }

    public void ChangeSideCardOrder()
    {
       SideCards = SideCards.OrderBy(o => o.visuals.GetSiblingIndex()).ToList();
    }

    public virtual void IncreaseSideCardDeck()
    { 
            
    }

    public virtual void AddCardDecorator(ICardDecorator decorator, object Caller)
    {
        cardDecorators.Add(new CardDecorateData(){ decorator = decorator, Caller = Caller});
    }

    public virtual void RemoveCardDecorator(object Caller) 
    {
        cardDecorators.RemoveAll(o => o.Caller == Caller);
    }

    public int CalculateLoss(int amount)
    {
        if (RoundMoney - amount <= 0)
        {
            return RoundMoney;
        }
        else
        {
            return amount;
        }
    }

    public void Deload()
    {
        foreach (var item in Hand)
        {
            item.Deload();
        }

        currDeck.Deload();
    }
}

