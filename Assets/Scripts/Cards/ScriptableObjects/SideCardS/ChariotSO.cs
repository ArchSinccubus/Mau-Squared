using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New ChariotSO")]
public class ChariotSO : HandChoiceSideSO, IObserverOnEndTurn
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnTurnEnd, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnTurnEnd, subscriber);
    }

    public IEnumerator TriggerOnEndTurn(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        EntityHandler entity = card.owner;

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(entity, CardMenuChoiceMode.Semi, o => o.Smoked, HandChoiceType.Clean));

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return entity.CleanCards(cd.result);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return card.owner.Hand.Where(o => o.Smoked).Count() > 0;
    }
}
