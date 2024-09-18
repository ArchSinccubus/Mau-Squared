using Assets.Scripts.Auxilary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New WildRideSO")]
public class WildRideSO : SideCardSO, IObserverOnCardPlayed, ICardDecorator
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardPlayed, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlayed, subscriber);
    }


    public HandCardData Decorate(object caller, HandCardData card)
    {
        SideCardDataHandler side = caller as SideCardDataHandler;

        if (side.TempData1 is float)
        {
            card.Mult *= (float)side.TempData1;
        }
        return card;
    }

    public override void OnPickup(BaseCardDataHandler card)
    {
        card.owner.AddCardDecorator(this, card);

        base.OnPickup(card);
    }

    public override void OnRemove(BaseCardDataHandler card)
    {
        card.owner.RemoveCardDecorator(card);

        base.OnRemove(card);
    }

    public IEnumerator TriggerOnCardPlayed(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        if (card.TempData1 == null)
        {
            card.TempData1 = 1f;
        }

        card.TempData1 = (float)card.TempData1 + MultAmount;

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        return card.owner == data.owner;
    }
}
