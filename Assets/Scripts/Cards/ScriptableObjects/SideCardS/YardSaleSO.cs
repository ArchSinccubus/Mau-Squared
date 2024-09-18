using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New YardSaleSO")]
public class YardSaleSO : SideCardSO, IObserverOnSideSell
{
    public override bool RoundEvents => false;
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnSellSide, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnSellSide, subscriber);
    }

    public IEnumerator TriggerOnSideSell(EventDataArgs args)
    {
        ShopLogicHandler shop = args.Caller as ShopLogicHandler;
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        HandCardDataHandler data = shop.MakeRandomCardFree();

        yield return shop.StartCoroutine(data.visuals.Pop());
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        ShopLogicHandler shop = args.Caller as ShopLogicHandler;

        return shop.CanMakeCardFree();
    }
}
