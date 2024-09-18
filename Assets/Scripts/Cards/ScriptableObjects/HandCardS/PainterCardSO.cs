using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New PainterCardSO")]
public class PainterCardSO : HandCardSO, IObserverOnCardPlayed
{
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

        card.SetTempColors(data.returnUnmodifiedData().cardColors, true);

        Debug.Log("Copycat card changed color to" + card.returnUnmodifiedData().cardColors + "!");

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        return card.owner == data.owner && card != data && (card as HandCardDataHandler).state == HandCardState.InHand && !data.returnUnmodifiedData().cardColors.Contains(Colors.None) && base.CanTrigger(card, args, EventType);
    }

    public override bool Playable(HandCardDataHandler card, HandCardDataHandler cardCompare)
    {
        return base.Playable(card, cardCompare) && card.ReturnTempMainColor() != Colors.None;
    }
}
