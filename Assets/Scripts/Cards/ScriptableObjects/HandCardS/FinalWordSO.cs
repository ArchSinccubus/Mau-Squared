using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New Round_Events_Hand_Card1")]
public class FinalWordSO : HandCardSO, IObserverOnEndRound
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRoundEnd, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRoundEnd, subscriber);
    }

    public IEnumerator TriggerOnEndRound(EventDataArgs args)
    {
        HandCardDataHandler thisCard = args.Sender as HandCardDataHandler;

        yield return thisCard.owner.AddMoney(MoneyAmount);
        
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler thisCard = args.Sender as HandCardDataHandler;
        EntityHandler entity = (args.Caller as RoundLogicManager).Winner;
        HandCardDataHandler data = ((EndRoundData)args.Data).endCard;

        return thisCard == data && thisCard.owner == entity;
    }
}
