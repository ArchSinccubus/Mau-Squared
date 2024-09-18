using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New SmokeFireSO")]
public class SmokeFireSO : HandCardSO, IObserverOnDraw
{
    public override bool overrideValue => true;

    public override bool overrideScore => true;

    public override int Score => 10;

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
        HandCardDataHandler card = args.Sender as HandCardDataHandler;
        List<HandCardDataHandler> cards = args.Data as List<HandCardDataHandler>;

        foreach (var item in cards)
        {
            if (item.Smoked)
            {
                card.SetMultScore(MultAmount);
                yield return card.owner.TriggerCard(card);
            }
        }
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;

        return entity == card.owner && (card as HandCardDataHandler).state == HandCardState.InHand && base.CanTrigger(card, args, EventType);
    }

}
