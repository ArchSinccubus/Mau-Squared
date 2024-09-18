using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New RecklessSpendingSO")]
public class RecklessSpendingSO : HandCardSO, IObserverOnMoneyChanged
{
    public override bool RoundEvents => false;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnMoneyChanged, subscriber, this);

    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnMoneyChanged, subscriber);
    }

    public IEnumerator TriggerOnMoneyChanged(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        card.AddScore(ScoreAmount);

        yield return new WaitForGameEndOfFrame();
    }

    public override void OnPickup(BaseCardDataHandler card)
    {
        Subscribe(card);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        int diff = (int)args.Data;

        return diff < 0 && GameManager.currRun.runState == GameState.InShop && base.CanTrigger(card, args, EventType);
    }
}
