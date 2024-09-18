using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New EarlyBirdSO")]
public class EarlyBirdSO : HandCardSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        if (GameManager.Round.Pile.Count == 0)
        {
            yield return card.owner.AddMoney(MoneyAmount);
        }
    }
}
