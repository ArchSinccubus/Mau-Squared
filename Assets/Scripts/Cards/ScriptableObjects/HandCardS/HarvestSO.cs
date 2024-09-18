using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New HarvestSO")]
public class HarvestSO : HandCardSO, IObserverOnBuy
{
    public override bool RoundEvents => false;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnBuy, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnBuy, subscriber);
    }

    public IEnumerator TriggerOnBuy(EventDataArgs args)
    {
        PlayerHandler player = GameManager.currRun.player;
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        card.AddScore(Score);

        yield return new WaitForGameEndOfFrame();
    }

    public override void OnPickup(BaseCardDataHandler card)
    {
        Subscribe(card);
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        BaseCardDataHandler data = args.Data as BaseCardDataHandler;

        return data is HandCardDataHandler;
    }
}
