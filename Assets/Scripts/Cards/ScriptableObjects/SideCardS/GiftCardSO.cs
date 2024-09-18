using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New GiftCardSO")]
public class GiftCardSO : SideCardSO,  IObserverOnEndRound
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRoundEnd, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibraryLater(DictionaryTypes.OnRoundEnd, subscriber);
    }

    public IEnumerator TriggerOnEndRound(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        card.owner.IncreaseSideCardDeck();

        yield return card.visuals.Vanish();

        card.owner.SideCards.Remove(card);
        card.owner.visuals.RemoveFromSideCards(card.visuals);

        card.temp = true;

        card.ClearForRound();

    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;

        return card.owner == entity && GameManager.currRun.roundScene.Winner == entity;
    }
}
