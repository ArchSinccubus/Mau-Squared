using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New StudentCardSO")]
public class CardinalSO : HandCardSO, IObserverOnDraw
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
        HandCardDataHandler card = args.Sender as HandCardDataHandler;
        List<HandCardDataHandler> data = args.Data as List<HandCardDataHandler>;

        foreach (var item in data)
        {
            if (!item.returnTempData().cardColors.Contains(Colors.Red) && item != card)
            {
                card.AddTempScore(ScoreAmount);

                yield return card.owner.TriggerCard(card);
            }
        }
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        List<HandCardDataHandler> data = args.Data as List<HandCardDataHandler>;

        return entity == card.owner && (card as HandCardDataHandler).state == HandCardState.InHand && !(data.Count == 1 && data.Contains(card)) && base.CanTrigger(card, args, EventType);
    }
}
