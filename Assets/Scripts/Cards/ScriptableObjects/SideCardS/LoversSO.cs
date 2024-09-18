using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New LoversSO")]
public class LoversSO : SideCardSO, IObserverOnCardPlayed
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
        HandCardDataHandler data = args.Data as HandCardDataHandler;
        
        data.SetTempMultScore(MultAmount);

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        HandCardDataHandler data = args.Data as HandCardDataHandler;
        HandCardDataHandler top = GameManager.Round.ReturnTopCardData();

        if (top != null)
        {
            return data.owner == card.owner && top.returnUnmodifiedData().cardColors.All(data.returnUnmodifiedData().cardColors.Contains) &&
            top.returnUnmodifiedData().cardValues.All(data.returnUnmodifiedData().cardValues.Contains);
        }
        return false;
    }
}
