using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New EmpressSO")]
public class EmpressSO : SideCardSO, IObserverOnDraw
{
    public override bool Tarot => true;

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
            List<Colors> AddedColors = new List<Colors>();

            var values = Enum.GetValues(typeof(Colors));
            AddedColors.Add((Colors)values.GetValue(GameManager.currRun.RoundRand.NextInt(0, 4)));

            if (item.data.Tarot)
            {
                AddedColors.Add((Colors)values.GetValue(GameManager.currRun.RoundRand.NextInt(0, 4)));
            }

            yield return item.visuals.Wiggle();

            yield return item.visuals.Flip(false);

            item.SetTempColors(AddedColors, false);

            yield return item.visuals.Flip(item.owner.IsPlayer);
        }        
    }
    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        EntityHandler entity = args.Caller as EntityHandler;

        return card.owner == entity;
    }
}
