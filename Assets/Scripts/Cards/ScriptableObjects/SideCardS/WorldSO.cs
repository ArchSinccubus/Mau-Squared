using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New WorldSO")]
public class WorldSO : SideCardSO, IObserverOnEndRound
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

        yield return card.owner.AddScore(card.owner.currDeck.DeckBase.Where(o => o.data.Tarot).Count() * ScoreAmount);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return true;
    }
}
