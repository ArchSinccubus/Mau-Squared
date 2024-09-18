using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New CardDrawSO")]
public class CardDrawSO : HandCardSO
{

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        yield return GameManager.Round.GetOpponent(card.owner).Draw(NumberAmount);
    }
}
