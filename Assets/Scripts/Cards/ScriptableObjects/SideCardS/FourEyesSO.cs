using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New FourEyesSO")]
public class FourEyesSO : SideCardSO, IObserverOnActivateSideCard
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

        data.RefreshCard();

        card.Used = true;

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        SideCardDataHandler data = args.Data as SideCardDataHandler;

        return data.owner == entity && !(card as SideCardDataHandler).Used;
    }
}
