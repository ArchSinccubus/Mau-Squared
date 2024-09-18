using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New GreenEnergySO")]
public class GreenEnergySO : HandCardSO, IObserverOnRecycle
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
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        card.AddScore(ScoreAmount);

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        DiscardEventArgs data = (DiscardEventArgs)args.Data;

        return data.cards.Where(o => o == card).Count() > 0 && base.CanTrigger(card, args, EventType);
    }
}
