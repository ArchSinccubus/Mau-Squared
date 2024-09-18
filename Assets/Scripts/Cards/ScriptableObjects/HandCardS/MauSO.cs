using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New MauSO")]
public class MauSO : HandCardSO, IObserverOnEndRound
{
    public override bool overrideScore => true;
    public override int Score => 200;

    public override bool overrideValue => true;

    public override bool overrideMult => true;

    public override float Mult => 2f;

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
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        if (card.Smoked)
        {
            if (card.owner == GameManager.currRun.player)
                GameManager.currRun.MauPunishE = true;
            else
            {
                GameManager.currRun.MauPunishP = true;
            }
        }
        else
            switch (card.state)
            {
                case HandCardState.InDeck:
                    yield return card.owner.AddMoney(MoneyAmount);
                    break;
                case HandCardState.InPile:
                    yield return card.owner.AddScore(ScoreAmount);
                    break;
                case HandCardState.InHand:
                    if (card.owner == GameManager.currRun.player)
                        GameManager.currRun.MauCardP++;
                    else
                    {
                        GameManager.currRun.MauCardE++;
                    }
                    break;
                default:
                    break;
            }
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return base.CanTrigger(card, args, EventType);
    }
}
