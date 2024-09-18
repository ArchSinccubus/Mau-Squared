using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New EmpressSO")]
public class EmpressSO : SideCardSO, IObserverOnDraw
{
    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnDraw, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnDraw, subscriber);
    }

    public IEnumerator TriggerOnDraw(EventDataArgs args)
    {
        List<HandCardDataHandler> data = args.Data as List<HandCardDataHandler>;

            foreach (var item in data)
            {
                var values = Enum.GetValues(typeof(Colors));
                Colors color = (Colors)values.GetValue(GameManager.currRun.RoundRand.NextInt(0, 4));


                yield return item.visuals.Wiggle();

                yield return item.visuals.Flip(false);

                item.SetTempColors(new List<Colors>() { color }, false);

                yield return item.visuals.Flip(item.owner.IsPlayer);
            }        
    }
    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;

        return card.owner == entity;
    }
}
