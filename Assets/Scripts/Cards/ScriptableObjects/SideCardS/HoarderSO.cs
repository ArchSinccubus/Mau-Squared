using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New HoarderSO")]
public class HoarderSO : SideCardSO, IObserverOnDraw
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnDraw, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnDraw, subscriber);
    }

    public IEnumerator TriggerOnDraw(EventDataArgs args)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        List<HandCardDataHandler> cards = args.Data as List<HandCardDataHandler>;

        yield return entity.AddScore(cards.Count * ScoreAmount);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;

        return card.owner == entity;
    }

}
