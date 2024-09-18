using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New RulesCardSO")]
public class RulesCardSO : SideCardSO, IPlayerDecorator, IObserverOnPreStartRound, IObserverOnEndRound
{
    public override bool SilentTrigger => true;

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

    public PlayerData Decorate(PlayerData player)
    {
        player.OutOfOptionsDrawAmount += 1;

        return player;
    }

    public override void OnPickup(BaseCardDataHandler card)
    {
        card.owner.playerDecorators.Add(this);

        base.OnPickup(card);
    }

    public override void OnRemove(BaseCardDataHandler card)
    {
        card.owner.playerDecorators.Remove(this);

        base.OnRemove(card);
    }

    public IEnumerator TriggerPreStartRound(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        EntityHandler entity = card.owner;
        EntityHandler target = GameManager.Round.GetOpponent(entity);

        target.playerDecorators.Add(this);

        yield return new WaitForGameEndOfFrame();
    }

    public IEnumerator TriggerOnEndRound(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;
        EntityHandler entity = card.owner;
        EntityHandler target = GameManager.Round.GetOpponent(entity);

        target.playerDecorators.Remove(this);

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return true;
    }
}
