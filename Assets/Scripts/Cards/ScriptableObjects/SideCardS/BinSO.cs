using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New BinSO")]
public class BinSO : SideCardSO, IObserverOnRecycle
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRecycle, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRecycle, subscriber);
    }

    public IEnumerator TriggerOnRecycle(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        EntityHandler Entity = args.Caller as EntityHandler;

        yield return Entity.AddScore(ScoreAmount);       
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler Entity = args.Caller as EntityHandler;

        return card.owner == Entity;
    }
}
