using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New FoolSO")]
public class FoolSO : SideCardSO, IObserverOnStartRound
{
    public override bool Tarot => true;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRoundStart, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRoundStart, subscriber);
    }

    public IEnumerator TriggerStartRound(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        EntityHandler entity = card.owner;

        bool hasTarot = entity.Hand.FirstOrDefault(o => o.data.Tarot) != null;

        yield return entity.AddScore(hasTarot ? ScoreAmount * 2 : ScoreAmount);
    }
    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return true;
    }
}
