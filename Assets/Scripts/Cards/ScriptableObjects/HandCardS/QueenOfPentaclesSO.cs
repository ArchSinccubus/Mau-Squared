using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New QueenOfPentaclesSO")]
public class QueenOfPentaclesSO : HandChoiceBaseSO, IObserverOnEndRound, IObserverOnMoneyChanged
{
    public override bool Tarot => true;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        yield return card.owner.ReduceMoney(MoneyAmount);
    }
    public IEnumerator TriggerOnMoneyChanged(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        card.TempData1 = (int)args.Data;

        yield return new WaitForGameEndOfFrame();
    }

    public IEnumerator TriggerOnEndRound(EventDataArgs args)
    {
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        EntityHandler entity = args.Sender as EntityHandler;

        yield return entity.AddMoney((int)card.TempData1 * 2);
        card.TempData1 = null;
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        if (EventType == DictionaryTypes.OnMoneyChanged)
        {
            return card == args.Sender && GameManager.currRun.runState == GameState.InRound;
        }
        else if(EventType == DictionaryTypes.OnRoundEnd)
        {
            return card.owner == args.Caller && (int)card.TempData1 > 0;
        }

        return false;
    }
}
