using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New BribeSO")]
public class BribeSO : SideCardSO
{
    public override bool Clickable => true;

    public override IEnumerator DoCommand(SideCardDataHandler card)
    {
        yield return card.owner.Draw(NumberAmount);
        yield return card.owner.AddMoney(MoneyAmount);
    }
}
