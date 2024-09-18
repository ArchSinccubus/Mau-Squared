using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New BalancedSO")]
public class BalancedSO : HandCardSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        int diff = card.owner.Hand.Count - GameManager.Round.GetOpponent(card.owner).Hand.Count;

        if (diff > 0)
        {
            yield return GameManager.Round.GetOpponent(card.owner).Draw(Mathf.Abs(diff));
        }
        else
        {
            yield return card.owner.Draw(Mathf.Abs(diff));
        }
    }
}
