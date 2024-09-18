using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New AICardSO")]
public class AICardSO : SideCardSO, IObserverOnPreStartRound, IObserverOnEndRound
{
    public override bool SilentTrigger => base.SilentTrigger;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnPreRoundStart, subscriber, this);
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRoundEnd, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnPreRoundStart, subscriber);
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRoundEnd, subscriber);

    }

    public IEnumerator TriggerPreStartRound(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        EntityHandler entity = card.owner;

        int ID = entity.SideCards.IndexOf(card);

        if (ID + 1 < entity.SideCards.Count)
        {
            if (entity.SideCards[ID + 1].data.Copyable && !entity.SideCards[ID + 1].data.Clickable)
            {
                card.data = entity.SideCards[ID + 1].data;
                if (card.data is ICardDecorator)
                {
                    entity.AddCardDecorator(card.data as ICardDecorator, card);
                }
                if (card.data is IPlayerDecorator)
                {
                    entity.playerDecorators.Add(card.data as IPlayerDecorator);
                }

                entity.SideCards[ID + 1].data.Subscribe(card);
            }
        }

        yield return new WaitForGameEndOfFrame();

    }

    public IEnumerator TriggerOnEndRound(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        EntityHandler entity = card.owner;

        card.data.Unsubscribe(card);
        
        if (card.data is ICardDecorator)
        {
            entity.RemoveCardDecorator(card);
        }
        if (card.data is IPlayerDecorator)
        {
            entity.playerDecorators.Remove(card.data as IPlayerDecorator);
        }

        card.data = this;

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        if (EventType == DictionaryTypes.OnPreRoundStart)
        {
            EntityHandler entity = card.owner;

            int ID = entity.SideCards.IndexOf(card as SideCardDataHandler);

            return ID + 1 < entity.SideCards.Count;
        }
        else if (EventType == DictionaryTypes.OnRoundEnd)
        {
            return card.baseData != this;
        }

        return false;
    }
}
