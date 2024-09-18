using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New TradingBinderSO")]
public class TradingBinderSO : HandChoiceSideSO
{
    public override bool ChoiceCard => true;

    public string choiceTextAdd;
    public override bool Clickable => true;

    public override IEnumerator DoCommand(SideCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Forced, o => true, HandChoiceType.Smoke));

        yield return cd.coroutine;
        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }
        HandCardDataHandler choice = cd.result[0];

        if (choice != null)
        {
            card.owner.Hand.Remove(choice);
            card.owner.visuals.RemoveFromHand(choice.visuals);

            choice.ClearForRound();
            choice.Deload();
        }

        CoroutineWithData<HandCardDataHandler> cd2 = new CoroutineWithData<HandCardDataHandler>(GameManager.Round, makeCardChoice(card.owner));
        yield return cd2.coroutine;

        while (cd2.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        choice = cd2.result;

        if (choice != null)
        {
            choice.temp = false;
            choice.InitForRound(card.owner.IsPlayer);
            yield return card.owner.AddHandCard(choice);
        }
    }

    public IEnumerator makeCardChoice(EntityHandler entity)
    {
        HandCardSO[] list = GameManager.instance.AssetLibrary.FetchRandomHandCards(GameManager.currRun.RoundRand, NumberAmount);

        List<HandCardDataHandler> cards = new List<HandCardDataHandler>();

        foreach (HandCardSO card in list) 
        {
            cards.Add(new HandCardDataHandler(card, entity, true));
        }

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, entity.MakeChoice(cards.ToArray(), choiceTextAdd, 1, CardMenuChoiceMode.Forced, SetChoiceType.HandAdd));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            yield return cd.result[0];
        }
    }
}
