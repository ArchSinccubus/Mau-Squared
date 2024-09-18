using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New EmptyCanvasSO")]
public class EmptyCanvasSO : SideCardSO, ICardDecorator, IObserverOnCardPlayed
{

    public override bool RoundEvents => false;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardPlayed, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlayed, subscriber);
    }

    public HandCardData Decorate(object caller, HandCardData card)
    {
        SideCardDataHandler side = caller as SideCardDataHandler;

        if (side.TempData1 is List<Colors>)
        {
            if (IsActive((List<Colors>)side.TempData1))
            {
                card.PreWild = true;
            }
        }

        return card;
    }
    public override void OnPickup(BaseCardDataHandler card)
    {
        card.owner.AddCardDecorator(this, card);
        Subscribe(card);

        base.OnPickup(card);
    }

    public override void OnRemove(BaseCardDataHandler card)
    {
        card.owner.RemoveCardDecorator(card);
        Unsubscribe(card);

        base.OnRemove(card);
    }

    public IEnumerator TriggerOnCardPlayed(EventDataArgs args)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        SideCardDataHandler card = args.Sender as SideCardDataHandler;


            if (card.TempData1 == null)
            {
                card.TempData1 = new List<Colors>();
            }

            List<Colors> colors = data.returnUnmodifiedData().cardColors;

            foreach (var item in colors)
            {
                if (item != Colors.None && !((List<Colors>)card.TempData1).Contains(item))
                {
                    ((List<Colors>)card.TempData1).Add(item);
                    yield return card.TriggerCardEffect(item.ToString() + "!");
                }
            }
        
    }

    public bool IsActive(List<Colors> list)
    {
        return list.Contains(Colors.Red) && list.Contains(Colors.Green) && list.Contains(Colors.Blue) && list.Contains(Colors.Orange);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;

        return card.owner == entity;
    }
}
