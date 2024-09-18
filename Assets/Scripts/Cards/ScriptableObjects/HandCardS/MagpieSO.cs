using Assets.Scripts.Auxilary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New MagpieSO")]
public class MagpieSO : HandCardSO
{
    public override bool ChoiceCard => true;

    public string choiceText;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {

        CoroutineWithData<long> cd = new CoroutineWithData<long>(GameManager.Round, GetDeckCard(card.owner));

        yield return cd.coroutine;

        while (cd.result == 0)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result != 0)
        {
            HandCardDataHandler cardToAdd = card.owner.currDeck.FetchCard(cd.result);

            yield return card.owner.AddHandCard(cardToAdd);
        }
    }


    public IEnumerator GetDeckCard(EntityHandler entity)
    {
        List<HandCardDataHandler> list = new List<HandCardDataHandler>();

        list = entity.currDeck.DeckBase.Select(o => new HandCardDataHandler(o) { TempData1 = o.ID }).ToList();

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, entity.MakeChoice(list.OrderBy(o => o.returnUnmodifiedData().cardColors[0]).ToArray(), choiceText, 1, CardMenuChoiceMode.Open, SetChoiceType.HandAdd));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            yield return (long)cd.result[0].TempData1;
        }
    }
}
