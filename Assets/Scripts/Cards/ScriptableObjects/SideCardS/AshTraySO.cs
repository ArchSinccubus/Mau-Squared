using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New AshTraySO")]
public class AshTraySO : SideCardSO, IObserverOnSmokedPlayed
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnSmokedPlayed, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnSmokedPlayed, subscriber);
    }

    public IEnumerator TriggerOnSmokedPlayed(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        yield return card.owner.AddScore(ScoreAmount);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        return data.owner == card.owner;
    }
}
