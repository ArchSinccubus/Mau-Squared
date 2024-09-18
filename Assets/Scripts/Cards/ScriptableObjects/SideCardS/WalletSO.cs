using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New WalletSO")]
public class WalletSO : SideCardSO, IObserverOnMoneyChanged
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnMoneyChanged, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnMoneyChanged, subscriber);
    }



    public IEnumerator TriggerOnMoneyChanged(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        int diff = (int)args.Data;

        yield return card.owner.AddScore(diff);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        int diff = (int)args.Data;
        EntityHandler entity = args.Caller as EntityHandler;

        return card.owner == entity && diff > 0;
    }
}
