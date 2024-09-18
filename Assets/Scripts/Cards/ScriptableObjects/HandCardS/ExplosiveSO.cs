using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New ExplosiveSO")]
public class ExplosiveSO : HandCardSO, IObserverOnSmokedPlayed
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
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        card.AddScore(ScoreAmount);

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;
        EntityHandler entity = args.Caller as EntityHandler;

        return (card as HandCardDataHandler).state == HandCardState.InHand && entity == data.owner && base.CanTrigger(card, args, EventType);
    }
}
