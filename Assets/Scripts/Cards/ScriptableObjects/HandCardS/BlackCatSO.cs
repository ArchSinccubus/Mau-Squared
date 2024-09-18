using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New BlackCatSO")]
public class BlackCatSO : HandCardSO
{
    public override bool overrideMult => true;
    public override float Mult => MultAmount;
    public override bool overrideScore => true;

    public override bool overrideValue => true;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        HandCardDataHandler[] CardsToSmoke = card.owner.Hand.Where(o => o != card && card.data is not BlackCatSO).ToArray();

        yield return card.owner.SmokeCards(card.owner, CardsToSmoke);
    }

    public override int CalcScore(HandCardDataHandler card)
    {
        if (GameManager.ViewingCollection)
        {
            return 0;
        }

        HandCardDataHandler[] CardsToSmoke = card.owner.Hand.Where(o => o != card && card.data is not BlackCatSO).ToArray();

        int totalScore = 0;

        foreach (var item in CardsToSmoke)
        {
            totalScore += item.data.CalcScore(item);
        }

        return totalScore;
    }
}
