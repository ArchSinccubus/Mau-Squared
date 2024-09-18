using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New CopycatCard")]
public class CopycatCardSO : HandCardSO, IObserverOnCardPlayed
{
    public override bool overrideColor => false;
    public override bool Transformer => true;


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

        card.SetTempValues(data.returnModifiedData().cardValues, true);

        Debug.Log("Copycat card changed value to" + card.returnUnmodifiedData().cardValues[0] + "!");

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        if (card.owner == data.owner && (card as HandCardDataHandler).state == HandCardState.InHand && data.returnUnmodifiedData().cardValues.Count > 0 && card != data && base.CanTrigger(card, args, EventType))
        {
            return !data.returnUnmodifiedData().cardValues.Contains(0);
        }

        return false;
    }
}
