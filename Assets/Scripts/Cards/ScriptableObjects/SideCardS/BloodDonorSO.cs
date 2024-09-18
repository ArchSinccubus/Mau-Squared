using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New BloodDonorSO")]
public class BloodDonorSO : SideCardSO, IObserverOnCardPlayed
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
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        
        yield return card.owner.AddMoney(MoneyAmount);
        
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        return data.owner == card.owner && data.returnModifiedData().cardColors.Contains(Colors.Red);
    }
}
