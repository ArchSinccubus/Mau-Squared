using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New MoonSO")]
public class MoonSO : SideCardSO, IObserverOnDraw
{
    public override bool Tarot => true;

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
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        List<HandCardDataHandler> data = args.Data as List<HandCardDataHandler>;

        HandCardDataHandler[] SmokedCards = data.Where(o => o.Smoked).ToArray();

        if (SmokedCards.FirstOrDefault(o => o.data.Tarot) != null)
        {
            SmokedCards = card.owner.Hand.Where(o => o.Smoked).ToArray();
        }

        yield return card.owner.CleanCards(SmokedCards);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        List<HandCardDataHandler> data = args.Data as List<HandCardDataHandler>;

        return card.owner == entity && data.Where(o => o.Smoked).Count() > 0;
    }
}
