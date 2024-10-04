using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New IDCard")]
public class IDCard : SideCardSO, ICardDecorator, IObserverOnPreStartRound
{
    public override bool ChoiceCard => true;

    public string choiceText;
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

        if (side.TempData1 is Colors)
        {
            if (card.cardColors.Contains((Colors)side.TempData1))
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

        card.TempData1 = cd.result.returnUnmodifiedData().cardColors[0];
    }

    public IEnumerator ChooseColor(EntityHandler entity)
    {

        List<HandCardDataHandler> list = new List<HandCardDataHandler>();

        for (int i = 0; i < 4; i++)
        {
            Colors c = (Colors)i;

            HandCardDataHandler tempCard = new HandCardDataHandler(GameManager.instance.AssetLibrary.FetchHandCardTemplate(), null, true);

            tempCard.SetMainColor(c);
            tempCard.SetMainValue(0);

            list.Add(tempCard);

            tempCard.IsShowToolTip = false;
        }

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, entity.MakeChoice(list.ToArray(), choiceText, 1, CardMenuChoiceMode.Forced, SetChoiceType.PreColor));

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
