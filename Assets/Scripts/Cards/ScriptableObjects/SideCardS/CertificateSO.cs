using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New CertificateSO")]
public class CertificateSO : HandChoiceSideSO, IObserverOnCardPlayed, IObserverOnCardPlaced
{
    public bool trigger;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardPlayed, subscriber, this);
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnCardPlaced, subscriber, this);

    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlayed, subscriber);
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnCardPlaced, subscriber);
    }

    public IEnumerator TriggerOnCardPlayed(EventDataArgs args)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        card.TempBool = true;

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Semi, o => o.data.Playable(o, data), HandChoiceType.PlayExtra));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            card.TempData1 = cd.result;
            card.TempData2 = data;
        }
    }

    public IEnumerator TriggerOnCardPlaced(EventDataArgs args)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        HandCardDataHandler[] selections = card.TempData1 as HandCardDataHandler[];
        HandCardDataHandler played = card.TempData2 as HandCardDataHandler;

        for (int i = selections.Length - 1; i >= 0; i--)
        {
            if (trigger)
            {
                yield return selections[i].PlayCard();
            }
            else
            {
                yield return selections[i].PlayCardNoTrigger();
            }
        }
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        HandCardDataHandler data = args.Data as HandCardDataHandler;

        if (EventType == DictionaryTypes.OnCardPlayed)
        {
            return data.owner == card.owner && data.data is BasicCardSO && !card.TempBool;
        }
        else
        {
            HandCardDataHandler played = card.TempData2 as HandCardDataHandler;

            return data == played;
        }
    }
}
