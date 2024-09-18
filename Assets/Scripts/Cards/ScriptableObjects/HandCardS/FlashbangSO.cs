using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New FlashbangSO")]
public class FlashbangSO : HandChoiceBaseSO
{

    public override IEnumerator OnThisDrawn(HandCardDataHandler card)
    {
        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(target, CardMenuChoiceMode.Forced, o => !o.Smoked, HandChoiceType.Smoke));

        yield return cd.coroutine;
        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return target.SmokeCards(card.owner, cd.result);

    }
}
