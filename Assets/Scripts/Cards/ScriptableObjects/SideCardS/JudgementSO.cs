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

        yield return data.cause.SmokeCards(data.cause, GetCardsToSmoke(data.cause).ToArray());      
    }

    public List<HandCardDataHandler> GetCardsToSmoke(EntityHandler entity)
    {
        List<HandCardDataHandler> cardsToSmoke = new List<HandCardDataHandler>();

        cardsToSmoke = GameManager.currRun.RoundRand.GetRandomElements(entity.Hand.Where(c => !c.Smoked).ToList(), NumberAmount);

        return cardsToSmoke;
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        SmokeEventData data = (SmokeEventData)args.Data;

        return card.owner == entity && data.cause != entity;
    }
}
