using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New SplitSecondSO")]
public class SplitSecondSO : SideCardSO, IObserverOnCardPlayed
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardPlayed, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlayed, subscriber);
    }

    public IEnumerator TriggerOnCardPlayed(EventDataArgs args)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        IEnumerable<SideCardDataHandler> Sides = card.owner.SideCards.Where(o => o.data.Clickable && o.Used);


        SideCardDataHandler side = GameManager.currRun.RoundRand.GetRandomElements(Sides.ToList())[0];

        side.RefreshCard();
        card.Used = true;
        yield return side.visuals.Pop();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;
        IEnumerable<SideCardDataHandler> Sides = card.owner.SideCards.Where(o => o.data.Clickable && o.Used);

        return data.owner == card.owner && data.returnModifiedData().cardValues.Contains(2) && Sides.Count() > 0 && !(card as SideCardDataHandler).Used;
    }
}
