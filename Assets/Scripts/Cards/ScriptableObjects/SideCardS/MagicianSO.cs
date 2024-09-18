using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New MagicianSO")]
public class MagicianSO : HandChoiceSideSO
{
    public override bool Clickable => true;

    public override IEnumerator DoCommand(SideCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Semi, o => true, HandChoiceType.Smoke));

        card.TempBool = true;
        yield return cd.coroutine;

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
