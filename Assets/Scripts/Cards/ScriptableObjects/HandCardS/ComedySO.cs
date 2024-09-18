using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New ComedySO")]
public class ComedySO : HandChoiceBaseSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Open, o => true, HandChoiceType.Upgrade));

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        if (cd.result.Length > 0)
        {
            HandCardDataHandler chosenCard = cd.result[0];

            yield return chosenCard.TriggerCardEffect("Triple!");

            List<int> list = chosenCard.returnUnmodifiedData().cardValues;

            if (list.FirstOrDefault() == 3)
            {
                chosenCard.SetMultScore(MultAmount);
            }
            else
            {
                chosenCard.SetTempMultScore(MultAmount);
            }
        }
    }

}
