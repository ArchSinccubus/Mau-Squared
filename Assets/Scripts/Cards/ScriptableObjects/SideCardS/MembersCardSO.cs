using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New MembersCardSO")]
public class MembersCardSO : SideCardSO, ICardDecorator, IObserverOnPreStartRound, IObserverOnEndRound
{
    public override bool ChoiceCard => true;

    public override bool SilentTrigger => true;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnPreRoundStart, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnPreRoundStart, subscriber);
    }

    public HandCardData Decorate(object Caller, HandCardData card)
    {
        SideCardDataHandler side = Caller as SideCardDataHandler;

        if (side.TempData1 is int)
        {
            if (card.cardValues.Contains((int)side.TempData1))
            {
                card.Mult *= MultAmount;
            }
        }
        

        return card;
    }

    public override void OnPickup(BaseCardDataHandler card)
    {
        card.owner.AddCardDecorator(this, card);
        Subscribe(card);

        base.OnPickup(card);
    }

    public override void OnRemove(BaseCardDataHandler card)
    {
        card.owner.RemoveCardDecorator(card);
        Unsubscribe(card);

        base.OnRemove(card);
    }

    public IEnumerator TriggerPreStartRound(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        CoroutineWithData<HandCardDataHandler> cd = new CoroutineWithData<HandCardDataHandler>(GameManager.Round, ChooseColor(card.owner));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        card.TempData1 = cd.result.returnUnmodifiedData().cardValues[0];
    }
    public IEnumerator TriggerOnEndRound(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        card.TempData1 = null;

        yield return new WaitForGameEndOfFrame();
    }

    public IEnumerator ChooseColor(EntityHandler entity)
    {

        List<HandCardDataHandler> list = new List<HandCardDataHandler>();

        for (int i = 1; i <= 9; i++)
        {
            int num = i;

            HandCardDataHandler tempCard = new HandCardDataHandler(GameManager.instance.AssetLibrary.FetchHandCardTemplate(), null, true);

            tempCard.SetMainValue(num);
            tempCard.SetMainColor(Colors.None);

            list.Add(tempCard);

            tempCard.IsShowToolTip = false;
        }

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, entity.MakeChoice(list.ToArray(), ReturnChoiceText(), 1, CardMenuChoiceMode.Forced, SetChoiceType.PreValue));

        yield return cd.coroutine;


        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return cd.result[0];
    }

    public override bool CanTrigger(BaseCardDataHandler card, EventDataArgs args, DictionaryTypes EventType)
    {
        return true;
    }
}
