using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New IncenseSO")]
public class IncenseSO : HandChoiceBaseSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Semi, o => o.Smoked, HandChoiceType.PlayExtra));

        yield return cd.coroutine;
        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }
        if (cd.result.Length > 0)
        {
            card.TempData1 = cd.result[0];
        }
    }

    public override IEnumerator OnThisPlaced(HandCardDataHandler card)
    {
        if (card.TempData1 is HandCardDataHandler)
        {
            yield return (card.TempData1 as HandCardDataHandler).PlayCard();

            card.TempData1 = null;
        }
    }
}
