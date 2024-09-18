using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New RecycleCardSO")]
public class RecycleCardSO : HandCardSO
{
    public override bool overrideScore => true;

    public override int Score => Mathf.FloorToInt(base.Score * MultAmount);

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        yield return card.owner.RecycleCards(card.owner.Hand.ToArray());
    }
}
