using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New RemoveVoucherSO")]
public class RemoveVoucherSO : SideCardSO, IObserverOnShopEnter
{
    public override bool RoundEvents => false;
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnShopEnter, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnShopEnter, subscriber);
    }


    public IEnumerator TriggerOnShopEnter(EventDataArgs args)
    {
        ShopLogicHandler shop = args.Data as ShopLogicHandler;

        shop.FreeRemove = true;

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return true;
    }
}
