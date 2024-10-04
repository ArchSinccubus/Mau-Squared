using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New PageOfPentaclesSO")]
public class PageOfPentaclesSO : HandCardSO, IObserverOnMoneyChanged
{
    public override bool Tarot => true;

    public IEnumerator TriggerOnMoneyChanged(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        card.AddScore(ScoreAmount);

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return card.owner == args.Caller && (int)args.Data > 0 && (card as HandCardDataHandler).state == HandCardState.InHand;
    }
}
