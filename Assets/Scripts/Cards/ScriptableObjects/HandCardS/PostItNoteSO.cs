using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New PostItNoteSO")]
public class PostItNoteSO : HandCardSO, IObserverOnCardPlayed
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardPlayed, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlayed, subscriber);
    }

    public IEnumerator TriggerOnCardPlayed(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;
        HandCardDataHandler data = args.Data as HandCardDataHandler;
        EntityHandler entity = args.Caller as EntityHandler;

        if (card.state == HandCardState.InHand && card.owner == data.owner)
        {
            if (data.returnModifiedData().cardValues.Count > 0)
            {
                card.AddTempScore(data.returnModifiedData().cardValues[0]);

                yield return entity.TriggerCard(card);
            }
        }
    }
}
