using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New JudgementSO")]
public class JudgementSO : SideCardSO, IObserverOnCardSmoked
{
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
        SmokeEventData data = (SmokeEventData)args.Data;
        bool HasTarot = data.cards.FirstOrDefault(o => o.data.Tarot) != null;
        yield return data.cause.SmokeCards(data.cause, GetCardsToSmoke(data.cause, HasTarot).ToArray());      
    }

    public List<HandCardDataHandler> GetCardsToSmoke(EntityHandler entity, bool all)
    {
        List<HandCardDataHandler> cardsToSmoke = new List<HandCardDataHandler>();

        if (!all)
        {
            cardsToSmoke = GameManager.currRun.RoundRand.GetRandomElements(entity.Hand.Where(c => !c.Smoked).ToList(), NumberAmount);
        }
        else
        {
            cardsToSmoke = entity.Hand;
        }

        return cardsToSmoke;
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        SmokeEventData data = (SmokeEventData)args.Data;

        return card.owner == entity && data.cause != entity;
    }
}
