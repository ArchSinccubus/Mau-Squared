using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New PerfumeSO")]
public class PerfumeSO : HandCardSO, IObserverOnCardCleaned, IObserverOnStartRound
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRoundStart, subscriber, this);
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardCleared, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRoundStart, subscriber);
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardCleared, subscriber);
    }

    public IEnumerator TriggerOnCleaned(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;
        EntityHandler entity = card.owner;
        List<HandCardDataHandler> cards = args.Data as List<HandCardDataHandler>;

        if (cards.Contains(card) && card.state == HandCardState.InHand)
        {
            card.MultTempMultScore(MultAmount);

            yield return entity.TriggerCard(card);
            
        }
    }

    public IEnumerator TriggerStartRound(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        card.Smoked = true;

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        List<HandCardDataHandler> cards = args.Data as List<HandCardDataHandler>;

        if (EventType == DictionaryTypes.OnCardCleared)
        {
            return cards.Contains(card) && base.CanTrigger(card, args, EventType);
        }
        else
            return true;
    }
}
