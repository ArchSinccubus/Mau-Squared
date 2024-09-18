using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New BlorboSO")]
public class BlorboSO : SideCardSO
{
    public string choiceText;
    public override bool Clickable => true;

    public override IEnumerator DoCommand(SideCardDataHandler card)
    {
        CoroutineWithData<long> cd = new CoroutineWithData<long>(GameManager.Round, ChooseCard(card.owner));

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

        yield return card.owner.Draw(1);
    }

    public IEnumerator ChooseCard(EntityHandler entity)
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
