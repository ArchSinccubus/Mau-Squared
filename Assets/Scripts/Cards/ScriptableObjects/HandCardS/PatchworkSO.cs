using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New PatchworkSO")]
public class PatchworkSO : HandCardSO, IObserverOnCardPlayed, IObserverOnEndRound
{
    public override int Score => 0;

    public override bool overrideValue => true;

    public override bool overrideScore => true;

    public override bool Transformer => true;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardPlayed, subscriber, this);
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRoundEnd, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlayed, subscriber);
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRoundEnd, subscriber);
    }

    public IEnumerator TriggerOnCardPlayed(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        data.data.Subscribe(card);

        card.SetTempColors(data.returnTempData().cardColors, true);
        card.SetTempValues(data.returnTempData().cardValues, true);
        card.SetTempScore(data.returnTempData().Score);
        card.SetTempMultScore(data.returnTempData().Mult);
        card.SetTempData(data.data.Name);

        card.data = data.data;

        yield return new WaitForGameEndOfFrame();
    }

    public IEnumerator TriggerOnEndRound(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        card.data = this;

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        if (EventType == DictionaryTypes.OnCardPlayed)
        {
            return card.owner == data.owner && (card as HandCardDataHandler).state == HandCardState.InHand && base.CanTrigger(card, args, EventType);
        }
        else
            return true;
    }
}
