using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New HermitSO")]
public class HermitSO : SideCardSO, IObserverOnCardPlayed
{
    public override bool Tarot => true;

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
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        data.SetTempMultScore(MultAmount);

        yield return card.TriggerCardEffect();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        HandCardDataHandler data = args.Data as HandCardDataHandler;
        HandCardDataHandler top = GameManager.Round.ReturnTopCardData();

        if (top != null)
        {
            return data.owner == card.owner && (data.returnModifiedData().cardValues.Intersect(top.returnUnmodifiedData().cardValues).Count() == 0 &&
            data.returnModifiedData().cardColors.Intersect(top.returnUnmodifiedData().cardColors).Count() == 0 || top.data.Tarot != card.baseData.Tarot);
        }
        return false;
    }
}
