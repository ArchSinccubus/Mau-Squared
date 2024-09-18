using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New ArtTheftSO")]
public class ArtTheftSO : HandCardSO, IObserverOnCardPlayed
{

    public override bool overrideColor => false;

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
        HandCardDataHandler card = args.Sender as HandCardDataHandler;
        HandCardDataHandler data = args.Data as HandCardDataHandler;
        EntityHandler entity = args.Caller as EntityHandler;

        //if (card.owner == data.owner && (card.returnModifiedData().cardColors.Intersect(data.returnModifiedData().cardColors).Count() > 0) && card.state == HandCardState.InHand)
        //{
        //    yield return entity.TriggerCard(card, "x" + MultAmount + "!");
            card.MultTempMultScore(MultAmount);
        //}

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        return card.owner == data.owner && (card.returnModifiedData().cardColors.Intersect(data.returnModifiedData().cardColors).Count() > 0) && ((HandCardDataHandler)card).state == HandCardState.InHand && base.CanTrigger(card, args, EventType);
    }
}
