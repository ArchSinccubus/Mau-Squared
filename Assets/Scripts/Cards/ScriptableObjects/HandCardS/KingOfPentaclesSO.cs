using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New KingOfPentaclesSO")]
public class KingOfPentaclesSO : HandCardSO
{
    public override bool overrideScore => true;

    public override int Score => 1000;



    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        yield return card.owner.SetMoney(0);
    }
}
