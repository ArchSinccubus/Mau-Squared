using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New RushJobSO")]
public class RushJobSO : PlusSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Semi, null, HandChoiceType.PlayExtra));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            card.TempData1 = cd.result;
        }
    }
}
