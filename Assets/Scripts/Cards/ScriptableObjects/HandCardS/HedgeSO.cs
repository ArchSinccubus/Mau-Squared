using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New HedgeSO")]
public class HedgeSO : HandChoiceBaseSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, GetExtraCards(card.owner, CardMenuChoiceMode.Semi, null, HandChoiceType.Recycle));

        CoroutineWaitForList list = new CoroutineWaitForList();

        List<HandCardDataHandler> cardsToSmoke = new List<HandCardDataHandler>();
        List<HandCardDataHandler> cardsToClean = new List<HandCardDataHandler>();

        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        foreach (var item in cd.result)
        {
            if (item.Smoked)
            {
                cardsToClean.Add(item);
            }
            else
            {
                cardsToSmoke.Add(item);
            }
        }

        yield return card.owner.SmokeCards(card.owner, cardsToSmoke.ToArray());
        yield return card.owner.CleanCards(cardsToClean.ToArray());

        yield return card.owner.RecycleCards(cd.result);
    }
}
