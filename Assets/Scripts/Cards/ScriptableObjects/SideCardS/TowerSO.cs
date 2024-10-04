using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New TowerSO")]
public class TowerSO : SideCardSO, IObserverOnCardSmoked
{
    public override bool Tarot => true;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardSmoked, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardSmoked, subscriber);
    }

    public IEnumerator TriggerOnSmoked(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        SmokeEventData data = (SmokeEventData)args.Data;

        int finalScore = 0;

        foreach (var item in data.cards)
        {
            finalScore += ScoreAmount;
            if (item.data.Tarot)
            {
                finalScore += ScoreAmount;
            }
        }

        yield return card.owner.AddScore(finalScore);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        SmokeEventData data = (SmokeEventData)args.Data;

        return data.cause == card.owner;
    }
}
