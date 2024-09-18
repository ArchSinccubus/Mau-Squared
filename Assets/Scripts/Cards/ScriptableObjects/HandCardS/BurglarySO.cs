using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New BurglarySO")]
public class BurglarySO : HandCardSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        EntityHandler target = GameManager.Round.GetOpponent(card.owner);

        int loss = target.CalculateLoss(card.returnModifiedData().cardValues[0]);

        yield return target.ReduceMoney(loss);

        yield return card.owner.AddMoney(loss);
    }
}
