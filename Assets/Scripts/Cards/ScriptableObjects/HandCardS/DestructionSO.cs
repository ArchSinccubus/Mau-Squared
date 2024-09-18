using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New DestructionSO")]
public class DestructionSO : HandCardSO, IObserverOnActivateSideCard
{
    public override int Score => 100;

    public override bool overrideScore => true;

    public override bool overrideValue => true;

    public override bool overrideColor => false;

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
        HandCardDataHandler card = args.Sender as HandCardDataHandler;
        EntityHandler entity = args.Caller as EntityHandler;
        EntityHandler target = GameManager.Round.GetOpponent(entity);

        HandCardDataHandler OpCard = target.Hand[GameManager.currRun.RoundRand.NextInt(0, target.Hand.Count)];
        HandCardDataHandler OwnerCard = entity.Hand[GameManager.currRun.RoundRand.NextInt(0, entity.Hand.Count)];

        yield return entity.SmokeCards(entity, OwnerCard, OpCard);

    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;
        SideCardDataHandler data = args.Data as SideCardDataHandler;


        return data.owner == entity && (card as HandCardDataHandler).state == HandCardState.InHand && data.data.Clickable && base.CanTrigger(card, args, EventType);
    }
}
