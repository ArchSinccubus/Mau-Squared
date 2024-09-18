using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New JusticeSO")]
public class JusticeSO : SideCardSO, IObserverOnCardSmoked
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
        SmokeEventData data = (SmokeEventData)args.Data;

        yield return data.cause.Draw(NumberAmount);
    }
    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        SmokeEventData data = (SmokeEventData)args.Data;

        return card.owner == entity && data.cause != entity;
    }
}
