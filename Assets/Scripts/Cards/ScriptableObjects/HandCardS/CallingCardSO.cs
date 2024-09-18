using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New CallingCardSO")]
public class CallingCardSO : HandChoiceBaseSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Forced, o => true, HandChoiceType.Smoke));


        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            HandCardDataHandler[] CardsToSmoke = cd.result.Where(o => !o.Smoked).ToArray();

            yield return card.owner.SmokeCards(card.owner, CardsToSmoke);

            yield return card.owner.RecycleCards(cd.result);
        }     
    }
}
