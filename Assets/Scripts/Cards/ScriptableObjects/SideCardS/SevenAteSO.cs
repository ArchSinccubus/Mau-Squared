using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New SevenAteSO")]
public class SevenAteSO : SideCardSO, IObserverOnRemove, ICardDecorator
{
    public override bool RoundEvents => false;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRemove, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRemove, subscriber);
    }

    public HandCardData Decorate(object caller, HandCardData card)
    {
        SideCardDataHandler side = caller as SideCardDataHandler;

        if (side.PermData1 is float)
        {
            card.Mult *= (float)side.PermData1;
        }

        return card;
    }
    public override void OnPickup(BaseCardDataHandler card)
    {
        card.owner.AddCardDecorator(this, card);

        base.OnPickup(card);
    }

    public override void OnRemove(BaseCardDataHandler card)
    {
        card.owner.RemoveCardDecorator(card);

        base.OnRemove(card);
    }

    public IEnumerator TriggerOnRemove(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        if (card.PermData1 == null) 
        {
            card.PermData1 = 1f;
        }
        
        card.PermData1 = (float)card.PermData1 + MultAmount;

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        return data.returnUnmodifiedData().cardValues.Count > 0;
    }
}
