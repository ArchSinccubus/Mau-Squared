using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New HandOfMidasSO")]
public class HandOfMidasSO : HandChoiceBaseSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Forced, o => !o.returnUnmodifiedData().cardColors.Contains(Colors.Orange), HandChoiceType.Transform));

        CoroutineWaitForList list = new CoroutineWaitForList();
        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }
        foreach (var item in cd.result)
        {
            GameManager.Round.StartCoroutine(list.CountCoroutine(FlipCard(item)));
        }

        yield return list;

        if (cd.result.Length > 0)
        {
            yield return card.owner.AddMoney(MoneyAmount);
        }
    }

    public IEnumerator FlipCard(HandCardDataHandler card)
    {

        if (card.owner.IsPlayer)
        {
            yield return card.visuals.Flip(false);
        }

        card.SetMainColor(Colors.Orange);

        if (card.owner.IsPlayer)
        {
            yield return card.visuals.Flip(card.owner.IsPlayer);
        }
        else
            yield return card.visuals.Wiggle();
    }
}
