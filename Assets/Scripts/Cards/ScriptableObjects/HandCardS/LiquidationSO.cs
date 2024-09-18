using Assets.Scripts.Auxilary;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New Liquidation")]
public class LiquidationSO : HandCardSO, IObserverOnRemove
{
    public override bool RoundEvents => false;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRemove, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRemove, subscriber);
    }

    public IEnumerator TriggerOnRemove(EventDataArgs args)
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
        return base.CanTrigger(card, args, EventType);
    }
}
