using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New LiveFastSO")]
public class LiveFastSO : HandCardSO
{
    public override int Score => 50;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        card.SetMultScore(MultAmount);

        yield return new WaitForGameEndOfFrame();
    }
}
