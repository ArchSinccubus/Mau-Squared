using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "New DynamoCard", menuName = "Mau/Cards/Hand/New DynamoCard")]
public class DynamoCardSO : HandCardSO, IObserverOnCardPlayed
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
        HandCardDataHandler card = args.Sender as HandCardDataHandler;

        card.AddScore(ScoreAmount);

        yield return new WaitForGameEndOfFrame();
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        return (card as HandCardDataHandler).state == HandCardState.InHand && card.owner == data.owner && data.returnModifiedData().cardColors.Contains(Colors.Red) && base.CanTrigger(card, args, EventType);
    }
}
