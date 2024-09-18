using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New PentacleSO")]
public class PentacleSO : LuckySevenSO
{
    public override int BasePrice => 1;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        yield return card.owner.AddMoney(1);
    }
}
