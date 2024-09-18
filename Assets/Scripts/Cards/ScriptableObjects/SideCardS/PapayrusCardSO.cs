using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New PapayrusCardSO")]
public class PapayrusCardSO : HandChoiceSideSO, IObserverOnStartTurn, IObserverOnDraw
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnTurnStart, subscriber, this);
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnDraw, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnTurnStart, subscriber);
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnDraw, subscriber);
    }

    public IEnumerator TriggerOnStartTurn(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        EntityHandler entity = args.Data as EntityHandler;


        ChoiceAmount = entity.Hand.Count;

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Semi, o => !o.Smoked, HandChoiceType.Smoke));

        card.TempBool = true;
        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            yield return card.owner.SmokeCards(card.owner, cd.result);
            yield return card.owner.RecycleCards(cd.result);
        }
        ChoiceAmount = 0;
    }

    public IEnumerator TriggerOnDraw(EventDataArgs args)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        List<HandCardDataHandler> data = args.Data as List<HandCardDataHandler>;

        Coroutine cor = null;

            HandCardDataHandler[] SmokedCards = data.Where(o => o.Smoked).ToArray();

            if (SmokedCards.Length > 0)
            {
                EntityHandler target = GameManager.Round.GetOpponent(entity);
                List<HandCardDataHandler> cardsToSmoke = new List<HandCardDataHandler>();
                cardsToSmoke = GameManager.currRun.RoundRand.GetRandomElements(target.currDeck.DeckBase.Where(c => !c.Smoked).ToList(), NumberAmount);
                cor = GameManager.Round.StartCoroutine(card.TriggerCardEffect());

                yield return target.SmokeCards(entity, cardsToSmoke.ToArray());

                yield return cor;
            }
        

    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        List<HandCardDataHandler> data = args.Data as List<HandCardDataHandler>;

        return card.owner == entity;
    }
}
