using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New RecyclingSO")]
public class RefreshmentsSO : HandCardSO, IObserverOnRecycle
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRecycle, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRecycle, subscriber);
    }

    public IEnumerator TriggerOnRecycle(EventDataArgs args)
    {
        DiscardEventArgs data = (DiscardEventArgs)args.Data;
        EntityHandler entity = args.Caller as EntityHandler;

        SideCardDataHandler[] Side = GameManager.currRun.RoundRand.GetRandomElements(entity.SideCards.Where(o => o.Used && o.data.Clickable).ToList(), NumberAmount).ToArray();
        if (Side.Length > 0)
        {
            yield return Side[0].visuals.Pop();
            Side[0].RefreshCard();
        }
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        DiscardEventArgs data = (DiscardEventArgs)args.Data;
        EntityHandler entity = args.Caller as EntityHandler;

        return data.owner == entity && data.cards.Contains(card) && entity.SideCards.Where(o => o.Used && o.data.Clickable).Count() > 0 && base.CanTrigger(card, args, EventType);
    }
}
