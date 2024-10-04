using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New SunSO")]
public class SunSO : SideCardSO, IObserverOnEndRound
{
    public override bool Tarot => true;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRoundEnd, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRoundEnd, subscriber);
    }

    public IEnumerator TriggerOnEndRound(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        int money = 0;

        foreach (var item in card.owner.Hand)
        {
            money += MoneyAmount;

            if (item.data.Tarot)
            {
                money += MoneyAmount;
            }
        }

        yield return card.owner.AddMoney(money);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return card.owner.Hand.Count > 0;
    }
}
