using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New DevilsAdvocateSO")]
public class DevilsAdvocateSO : HandCardSO
{
    public override bool overrideScore => true;

    public override int Score => 666;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        yield return card.owner.SetMoney(0);
    }
}
