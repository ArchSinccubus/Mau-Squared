using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New SaboutageSO")]
public class SaboutageSO : SideCardSO, IObserverOnStartRound
{
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
        EntityHandler target = GameManager.Round.GetOpponent(entity);
        List<HandCardDataHandler> cardsToSmoke = new List<HandCardDataHandler>();
        cardsToSmoke = GameManager.currRun.RoundRand.GetRandomElements(target.currDeck.DeckBase.Where(c => !c.Smoked).ToList(), NumberAmount).ToList();

        yield return target.SmokeCards(entity, cardsToSmoke.ToArray());
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return true;
    }
}
