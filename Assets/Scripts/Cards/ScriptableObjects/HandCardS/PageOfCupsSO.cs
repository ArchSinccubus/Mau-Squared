using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New PageOfCupsSO")]
public class PageOfCupsSO : HandChoiceBaseSO
{
    public override bool Tarot => true;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Open, o => !o.Smoked, HandChoiceType.Smoke));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            yield return card.owner.SmokeCards(card.owner, cd.result);

            float FinalMult = 1 + (MultAmount * cd.result.Length);

            card.MultTempMultScore(FinalMult);
        }
    }
}
