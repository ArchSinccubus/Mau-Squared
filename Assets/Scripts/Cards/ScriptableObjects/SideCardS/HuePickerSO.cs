using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New HuePickerSO")]
public class HuePickerSO : SideCardSO, IObserverOnPreStartRound, IObserverOnStartRound
{
    public override bool ChoiceCard => true;

    public string choiceText;

    public override bool SilentTrigger => true;

    public override void Subscribe(object subscriber)
    {
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnPreRoundStart, subscriber, this);
        ObserverManagerSystem.SubscribeToLibrary(DictionaryTypes.OnRoundStart, subscriber, this);
    }

    public override void Unsubscribe(object subscriber)
    {
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnPreRoundStart, subscriber);
        ObserverManagerSystem.UnsubscribeFromLibrary(DictionaryTypes.OnRoundStart, subscriber);
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

    public IEnumerator TriggerStartRound(EventDataArgs args)
    {
        SideCardDataHandler card = args.Sender as SideCardDataHandler;

        CoroutineWaitForList list = new CoroutineWaitForList();

        foreach (var item in card.owner.Hand)
        {
            GameManager.instance.StartCoroutine(list.CountCoroutine(changeCard(item, (Colors)card.TempData1)));
        }

        yield return list;
    }


    public IEnumerator changeCard(HandCardDataHandler card, Colors c)
    {
        yield return card.visuals.Flip(false);

        card.SetTempMainColor(c);
        yield return card.TriggerCardEffect();

        yield return card.visuals.Flip(card.owner.IsPlayer);
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
