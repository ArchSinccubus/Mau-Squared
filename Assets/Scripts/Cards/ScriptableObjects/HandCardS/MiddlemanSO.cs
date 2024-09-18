using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New MiddlemanSO")]
public class MiddlemanSO : HandCardSO, IObserverOnActivateSideCard
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnActivateSideCard, subscriber, this);
    }


    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnActivateSideCard, subscriber);
    }


    public IEnumerator TriggerOnActivateSideCard(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        yield return card.owner.AddMoney(MoneyAmount);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        return card.owner == data.owner && (card as HandCardDataHandler).state == HandCardState.InHand && base.CanTrigger(card, args, EventType);
    }
}
