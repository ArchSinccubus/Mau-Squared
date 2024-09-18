using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New MirageSO")]
public class MirageSO : SideCardSO, ICardDecorator, IObserverOnCardPlaced
{    
    public override bool Clickable => true;
    public override bool SilentTrigger => true;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardPlaced, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlaced, subscriber);
    }

    public override IEnumerator DoCommand(SideCardDataHandler card)
    {
        card.TempBool = true;
        card.TempData1 = (float)card.TempData1 * MultAmount;

        yield return new WaitForGameEndOfFrame();
    }

    public HandCardData Decorate(object caller, HandCardData card)
    {
        SideCardDataHandler side = caller as SideCardDataHandler;

        if (side.TempBool)
        {
            card.Mult *= (float)side.TempData1;
            
        }

        return card;
    }

    public override void OnPickup(BaseCardDataHandler card)
    {
        card.owner.AddCardDecorator(this, card);
        card.TempData1 = 1f;

        base.OnPickup(card);
    }

    public override void OnRemove(BaseCardDataHandler card)
    {
        card.owner.RemoveCardDecorator(card);

        base.OnRemove(card);
    }

    public IEnumerator TriggerOnCardPlaced(EventDataArgs args)
    {
        HandCardDataHandler card = args.Data as HandCardDataHandler;
        SideCardDataHandler side = args.Sender as SideCardDataHandler;

        if (card.owner == side.owner && side.TempBool)
        {
            side.TempBool = false;
            side.TempData1 = 1f;
        }

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler hand = args.Data as HandCardDataHandler;

        return hand.owner == card.owner && card.TempBool;
    }
}
