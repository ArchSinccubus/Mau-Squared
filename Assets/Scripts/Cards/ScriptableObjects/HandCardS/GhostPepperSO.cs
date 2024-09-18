using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/GhostPepperSO")]
public class GhostPepperSO : HandCardSO, IObserverOnCardPlaced, IObserverOnEndTurn
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardPlaced, subscriber, this);
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnTurnEnd, subscriber, this);

    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlaced, subscriber);
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnTurnEnd, subscriber);

    }
    public IEnumerator TriggerOnCardPlaced(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;
        HandCardDataHandler data = GameManager.Round.ReturnTopCardData();

        if (card.owner == data.owner && data.Smoked && card.state == HandCardState.InHand)
        {
            card.SetTempMultScore(MultAmount);

            card.TempBool = true;
            yield return card.PlayCard();
        }
    }

    public IEnumerator TriggerOnEndTurn(EventDataArgs args)
    {
        //Test this when you get home!

        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        if (card.TempBool)
        {
            card.ignore = false;
            card.TempBool = false;
        }
        yield return new WaitForGameEndOfFrame();
    }

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        if (card.TempBool)
        {
            card.ignore = true;
        }
        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = GameManager.Round.ReturnTopCardData();

        if (EventType == DictionaryTypes.OnCardPlaced)
        {
            return card.owner == data.owner && data.Smoked && (card as HandCardDataHandler).state == HandCardState.InHand && base.CanTrigger(card, args, EventType);
        }
        else
        {
            return true;
        }
    }

}
