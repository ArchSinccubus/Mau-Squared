using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New QueenOfCupsSO")]
public class QueenOfCupsSO : HandCardSO, IObserverOnCardSmoked
{
    public override bool Tarot => true;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardSmoked, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardSmoked, subscriber);
    }

    public IEnumerator TriggerOnSmoked(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;
        SmokeEventData data = (SmokeEventData)args.Data;
        EntityHandler entity = args.Caller as EntityHandler;

        foreach (var item in data.cards)
        {
            card.SetMultScore(MultAmount);

            yield return entity.TriggerCard(card);
        }
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return (card as HandCardDataHandler).state == HandCardState.InHand && base.CanTrigger(card, args, EventType);
    }
}
