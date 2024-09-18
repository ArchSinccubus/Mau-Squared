using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New LandTaxSO")]
public class LandTaxSO : HandChoiceBaseSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Open, o => !o.Smoked, HandChoiceType.Smoke));

        yield return cd.coroutine;
        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return card.owner.SmokeCards(card.owner, cd.result);


        int loss = target.CalculateLoss(cd.result.Length * MoneyAmount);

        yield return target.ReduceMoney(loss);

        yield return card.owner.AddMoney(loss);
    }
}
