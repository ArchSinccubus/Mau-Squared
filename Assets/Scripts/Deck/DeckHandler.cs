using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeckHandler
{
    public List<HandCardDataHandler> DeckBase;

    [JsonIgnore]
    public EntityHandler owner;

    [JsonIgnore]
    public IDeck visuals;

    public DeckHandler(EntityHandler owner, List<HandCardSO> deck)
    {
        this.owner = owner;

        DeckBase = new List<HandCardDataHandler>();

        foreach (var cardtemplate in deck)
        {
            HandCardDataHandler newCard = new HandCardDataHandler(cardtemplate, owner, false);

            //CardVisualHandler newCard = GameObject.Instantiate(GameManager.instance.cardPrefab);
            //newCard.init(cardtemplate);
            newCard.state = HandCardState.InDeck;
            DeckBase.Add(newCard);
        }
    }

    [JsonConstructor]
    public DeckHandler(EntityHandler owner, List<HandCardDataHandler> DeckBase)
    {
        this.owner = owner;

        this.DeckBase = DeckBase;

        foreach (var cardtemplate in DeckBase)
        {
            cardtemplate.owner = owner;

            //CardVisualHandler newCard = GameObject.Instantiate(GameManager.instance.cardPrefab);
            //newCard.init(cardtemplate);
            cardtemplate.state = HandCardState.InDeck;
        }
    }

    public void InitForRound(IDeck visualRef)
    { 
        visuals = visualRef;
        visuals.init(this);

        if (owner.IsPlayer)
        {
            visuals.DeckClick.SetDelegate(ViewDeck);
        }

        foreach (var item in DeckBase)
        {
            item.InitForRound(owner.IsPlayer);
            visuals.AddCardToDeck(item.visuals);
            item.visuals.SetPos(visuals.transform);

        }

        visuals.placeDeck(owner.visuals.transform);

    }

    public void LoadDeck(IDeck visualRef)
    {
        visuals = visualRef;
        visuals.init(this);

        if (owner.IsPlayer)
        {
            visuals.DeckClick.SetDelegate(ViewDeck);
        }

        foreach (var item in DeckBase)
        {
            item.InitForView(false, 0, false);
            visuals.AddCardToDeck(item.visuals);
            item.visuals.SetPos(visuals.transform);

        }

        visuals.placeDeck(owner.visuals.transform);
    }

    public bool CanDrawXCards(int num)
    {
        if (DeckBase.Count >= num)
        {
            return true;
        }
        return false;
    }

    public List<HandCardDataHandler> Draw(int num)
    {
        List<HandCardDataHandler> CardsToDraw = new List<HandCardDataHandler>();

        int limit = DeckBase.Count - num;

        for (int i = 0; i < num; i++)
        {
            CardsToDraw.Add(DrawACard());
        }

        //for (int i = DeckBase.Count - 1; i >= limit; i--)
        //{
        //    CardsToDraw.Add(DeckBase[i]);
        //    DeckBase.RemoveAt(i);
        //}
        //
        //foreach (var item in CardsToDraw)
        //{
        //    item.visuals.MoveCard(owner.location.HandLoc.transform);
        //    item.state = CardState.InHand;
        //}

        visuals.OrganizeDeck();

        return CardsToDraw;
    }

    public HandCardDataHandler DrawACard()
    {
        HandCardDataHandler newCard = DeckBase[DeckBase.Count - 1];
        DeckBase.Remove(newCard);
        visuals.DrawCardFromDeck(newCard.visuals);

        return newCard;
    }

    public HandCardDataHandler FetchCard(long ID)
    {
        HandCardDataHandler card = DeckBase.Where(o => o.ID == ID).FirstOrDefault();
        DeckBase.Remove(card);
        visuals.DrawCardFromDeck(card.visuals);

        return card;

    }

    public void RemoveCard(params HandCardDataHandler[] cards)
    {
        foreach (var item in cards)
        {
            DeckBase.Remove(item);
        }
        visuals.RemoveCardsFromDeck(getCardVisuals(cards));
    }

    public void AddCardToBottomAtSpot(int space, params HandCardDataHandler[] cards)
    {
        foreach (var item in cards)
        {
            item.state = HandCardState.InDeck;
            DeckBase.Add(item);
            HandCardDataHandler rep = DeckBase[space];
            DeckBase[space] = item;
            DeckBase[DeckBase.Count - 1] = rep;
            visuals.AddCardToDeck(item.visuals);
            item.data.OnPickup(item);
        }
            visuals.OrganizeDeck();
    }

    public void AddCardToShuffle(params HandCardDataHandler[] cards)
    {
        foreach (var item in cards)
        {
            item.state = HandCardState.InDeck;
            DeckBase.Add(item);
            visuals.AddCardToDeck(item.visuals);
        }
        Shuffle();
        visuals.OrganizeDeck();
    }

    public bool IsEmpty()
    {
        if (DeckBase.Count == 0)
        {
            return true;
        }
        return false;
    }

    public void Shuffle()
    {
        int n = DeckBase.Count;
        while (n > 1)
        {
            n--;
            int k = GameManager.currRun.RoundRand.NextInt(0, n + 1);
            HandCardDataHandler value = DeckBase[k];
            DeckBase[k] = DeckBase[n];
            DeckBase[n] = value;
        }
    }

    public void ViewDeck()
    {
        if (GameManager.currRun.player.CanAct)
        {
            HandCardDataHandler[] array = DeckBase.ConvertAll(o => o = new HandCardDataHandler(o)).Reverse<HandCardDataHandler>().ToArray();


            //This is the most scuffed up shit ever. I genuinely don't care at this point lmao
            if (owner.SideCards.Where(o => o.data is PriestessSO).Count() == 0 || GameManager.currRun.runState == GameState.InShop)
            {
                array = array.OrderBy(o => (o.returnUnmodifiedData().cardValues.Count > 0 ? o.returnUnmodifiedData().cardValues[0] : int.MaxValue)).ToArray();
                array = array.OrderBy(o => o.returnUnmodifiedData().cardColors[0]).ToArray();

                Debug.Log("Organizing cards to obfuscate deck order");
            }

            GameManager.Round.CardViewer.initMenu(array, "", true, CardMenuChoiceMode.Null, MenuViewMode.View, 4);
            GameManager.Round.CardViewer.OpenMenu();

            foreach (var item in array)
            {
                item.RevealCard();
            }
        }

    }

    public ICardVisuals[] getCardVisuals(HandCardDataHandler[] cards)
    {
        List<ICardVisuals> vis = new List<ICardVisuals>();

        foreach (var item in cards)
        {
            vis.Add(item.visuals);
        }

        return vis.ToArray();
    }

    public int GetMostCommonValue()
    {
        List<int> mainValues = new List<int>();
        foreach (var item in DeckBase)
        {
           mainValues.AddRange(item.returnModifiedData().cardValues);           
        }

        return mainValues.GroupBy(i => i).OrderByDescending(o => o.Count()).Select(i => i.Key).First();
    }

    public Colors GetMostCommonColor()
    {
        List<Colors> mainColors = new List<Colors>();
        foreach (var item in DeckBase)
        {
            mainColors.AddRange(item.returnModifiedData().cardColors.Where(o => o != Colors.None));
        }

        return mainColors.GroupBy(i => i).OrderByDescending(o => o.Count()).Select(i => i.Key).First();
    }

    public void Deload()
    {
        foreach (var item in DeckBase)
        {
            item.Deload();
        }

        DeckBase.Clear();
    }
}
