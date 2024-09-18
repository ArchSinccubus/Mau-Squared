using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New StrategySO")]
public class StrategySO : HandChoiceSideSO, IObserverOnActivateSideCard
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnActivateSideCard, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnActivateSideCard, subscriber);
    }

    public IEnumerator TriggerOnActivateSideCard(EventDataArgs args)
    {
        SideCardDataHandler data = args.Data as SideCardDataHandler;
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        EntityHandler entity = args.Caller as EntityHandler;

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Semi, o => true, HandChoiceType.Recycle));

        card.TempBool = true;
        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            yield return entity.RecycleCards(cd.result);
        }
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        SideCardDataHandler data = args.Data as SideCardDataHandler;

        return data.owner == card.owner && data != card;
    }
}
