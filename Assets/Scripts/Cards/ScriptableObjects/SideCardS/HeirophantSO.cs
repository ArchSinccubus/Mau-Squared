using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New HeirophantSO")]
public class HeirophantSO : SideCardSO, IObserverOnCardPlayed
{
    public override bool SilentTrigger => true;

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
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        data.AddScore(ScoreAmount);
        data.SetMultScore(MultAmount);

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        return data.data.GetType() == typeof(BasicCardSO) && card.owner == data.owner;
    }
}
