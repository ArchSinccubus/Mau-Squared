using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New HangedManSO")]
public class HangedManSO : SideCardSO, IObserverOnEndRound
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

        int score = 0;

        foreach (var item in card.owner.Hand)
        {
            score += ScoreAmount;

            if (item.data.Tarot)
            {
                score += ScoreAmount;
            }
        }

        yield return card.owner.AddScore(score);
    }


    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return true;
    }
}
