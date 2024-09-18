using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New SurfUpSO")]
public class SurfUpSO : HandCardSO, IObserverOnCardCleaned
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardCleared, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardCleared, subscriber);
    }

    public IEnumerator TriggerOnCleaned(EventDataArgs args)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        HandCardDataHandler card = args.Sender as HandCardDataHandler;
        List<HandCardDataHandler> cards = args.Data as List<HandCardDataHandler>;

        foreach (var item in cards)
        {
            card.AddScore(ScoreAmount);

            yield return entity.TriggerCard(card);

        }
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;

        return entity == card.owner && (card as HandCardDataHandler).state == HandCardState.InHand && base.CanTrigger(card, args, EventType);
    }
}
