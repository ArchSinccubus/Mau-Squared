using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New JusticeSO")]
public class JusticeSO : SideCardSO, IObserverOnCardSmoked
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
        SmokeEventData data = (SmokeEventData)args.Data;

        bool HasTarot = data.cards.FirstOrDefault(o => o.data.Tarot) != null;

        yield return data.cause.Draw(HasTarot ? ScoreAmount : NumberAmount);
    }
    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        SmokeEventData data = (SmokeEventData)args.Data;

        return card.owner == entity && data.cause != entity;
    }
}
