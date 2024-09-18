using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New CardioSO")]
public class CardioSO : HandChoiceBaseSO
{

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Semi, o => !o.Smoked, HandChoiceType.Smoke));


        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }
        if (cd.result.Length > 0)
        {
            yield return card.owner.SmokeCards(card.owner, cd.result);

            yield return card.owner.AddScore(cd.result.Length * ScoreAmount);
        }        
    }
}
