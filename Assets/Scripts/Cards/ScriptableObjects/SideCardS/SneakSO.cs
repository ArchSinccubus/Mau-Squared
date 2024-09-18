using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New SneakSO")]
public class SneakSO : SideCardSO, IPlayerDecorator, IObserverOnPreStartRound, IObserverOnEndRound
{
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
        player.StartHandSize += NumberAmount;

        return player;
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
