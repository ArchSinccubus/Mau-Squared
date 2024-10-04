using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New KingOfSwordsSO")]
public class KingOfSwordsSO : HandCardSO, IObserverOnActivateSideCard
{
    public override bool Tarot => true;

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
        HandCardDataHandler card = args.Sender as HandCardDataHandler;
        SideCardDataHandler data = args.Data as SideCardDataHandler;

        if (card.owner == data.owner)
        {
            card.SetMultScore(MultAmount);
        }

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;

        return entity == card.owner && (card as HandCardDataHandler).state == HandCardState.InHand && base.CanTrigger(card, args, EventType);
    }
}
