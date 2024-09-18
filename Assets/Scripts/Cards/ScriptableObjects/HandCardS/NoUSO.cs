using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New NoUSO")]
public class NoUSO : HandCardSO, IObserverOnCardSmoked
{
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
        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        yield return target.SmokeCards(card.owner, target.Hand.ToArray());       
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        SmokeEventData data = (SmokeEventData)args.Data;

        return data.cards.Contains(card) && data.cause != card.owner && base.CanTrigger(card, args, EventType);
    }
}
