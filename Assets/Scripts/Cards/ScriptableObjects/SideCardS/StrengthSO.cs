using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New StrengthSO")]
public class StrengthSO : SideCardSO, IObserverOnRecycle
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
        EntityHandler entity = args.Caller as EntityHandler;
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        DiscardEventArgs data = (DiscardEventArgs)args.Data;

        HandCardDataHandler[] CardsToClean = data.cards.Where(o => o.Smoked).ToArray();
        
        yield return card.owner.CleanCards(CardsToClean);   
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        DiscardEventArgs data = (DiscardEventArgs)args.Data;

        HandCardDataHandler[] CardsToClean = data.cards.Where(o => o.Smoked).ToArray();

        return CardsToClean.Length > 0;    
    }
}
