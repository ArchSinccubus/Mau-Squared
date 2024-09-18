using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New MulliganCard")]
public class MulliganCardSO : HandChoiceSideSO, IObserverOnStartTurn
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnTurnStart, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnTurnStart, subscriber);
    }

    public IEnumerator TriggerOnStartTurn(EventDataArgs args)
    {
        EntityHandler player = args.Data as EntityHandler;

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(player, CardMenuChoiceMode.Open, o => true, HandChoiceType.Recycle));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result != null)
        {
            yield return player.RecycleCards(cd.result);
        }
    }


    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        List<HandCardDataHandler> data = args.Data as List<HandCardDataHandler>;

        return card.owner == entity;
    }
}
