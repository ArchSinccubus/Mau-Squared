using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New TemperanceSO")]
public class TemperanceSO : SideCardSO, IObserverOnEndRound
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
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        
        yield return card.owner.AddMoney(MoneyAmount);        
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return !card.owner.ActivatedEffects;
    }

}
