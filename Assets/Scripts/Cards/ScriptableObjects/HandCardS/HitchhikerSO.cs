using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New HitchhikerSO")]
public class HitchhikerSO : HandCardSO, IObserverOnCardPlaced
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardPlaced, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlaced, subscriber);
    }

    public IEnumerator TriggerOnCardPlaced(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        yield return card.PlayCard();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        return card.owner == data.owner && card != data && data.returnModifiedData().cardColors.Contains(Colors.Green) && (card as HandCardDataHandler).state == HandCardState.InHand && base.CanTrigger(card, args, EventType);
    }
}
