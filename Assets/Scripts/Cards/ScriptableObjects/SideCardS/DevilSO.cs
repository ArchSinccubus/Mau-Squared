using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New DevilSO")]
public class DevilSO : SideCardSO, IObserverOnDraw
{
    public override bool Tarot => true;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnDraw, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnDraw, subscriber);
    }

    public IEnumerator TriggerOnDraw(EventDataArgs args)
    {
        EntityHandler entity = args.Caller as EntityHandler;

        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        yield return entity.AddMoney(MoneyAmount);

        if (card.data.Tarot)
        {
            card.SetTempWild();

            yield return entity.TriggerCard(card, "Wild!");
        }
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;

        return card.owner == entity && GameManager.Round.state == RoundState.DrawForTurn;
    }
}
