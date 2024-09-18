using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New ReportCardSO")]
public class ReportCardSO : SideCardSO, IObserverOnEndRound
{
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

        EntityHandler entity = card.owner;
        EntityHandler target = GameManager.Round.GetOpponent(entity);

        if (entity.GetScore() > target.GetScore())
        {
            yield return card.TriggerCardEffect();

            yield return entity.AddMoney(MoneyAmount);

        }
    }
}
