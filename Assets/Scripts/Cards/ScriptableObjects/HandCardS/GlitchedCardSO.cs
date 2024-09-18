using Assets.Scripts.Auxilary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New GlitchedCardSO")]
public class GlitchedCardSO : HandCardSO, IObserverOnCardPlayed
{
    public override bool overrideColor => false;
    public override bool overrideValue => false;

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
        HandCardDataHandler thisCard = args.Sender as HandCardDataHandler;

        var values = Enum.GetValues(typeof(Colors));
        Colors color = (Colors)values.GetValue(GameManager.currRun.RoundRand.NextInt(0, 4));

        int number = GameManager.currRun.RoundRand.NextInt(1, 10);

        thisCard.SetTempMainColor(color);
        thisCard.SetTempValues(new List<int> { number }, true);

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {

        return (card as HandCardDataHandler).state == HandCardState.InHand && base.CanTrigger(card, args, EventType);
    }
}
