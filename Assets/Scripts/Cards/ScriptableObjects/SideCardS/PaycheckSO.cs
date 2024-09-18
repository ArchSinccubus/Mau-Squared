using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New PaycheckSO")]
public class PaycheckSO : SideCardSO, IObserverOnStartRound
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRoundStart, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRoundStart, subscriber);
    }

    public IEnumerator TriggerStartRound(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        Coroutine cor = GameManager.Round.StartCoroutine(card.TriggerCardEffect());

        yield return card.owner.AddMoney(card.owner.Hand.Count);

        yield return cor;
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return true;
    }
}
