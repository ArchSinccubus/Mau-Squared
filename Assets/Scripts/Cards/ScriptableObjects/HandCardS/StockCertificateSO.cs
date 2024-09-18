using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New StockCertificateSO")]
public class StockCertificateSO : HandCardSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {

        yield return card.owner.Draw(1);


        HandCardDataHandler drawnCard = card.owner.Hand[card.owner.Hand.Count - 1];

        if (drawnCard.returnUnmodifiedData().cardValues.Count > 0)
        {
            yield return card.owner.AddMoney(drawnCard.returnTempData().cardValues[0] * MoneyAmount);
            yield return card.owner.TriggerCard(card);
        }

    }
}
