using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New NailCardSO")]
public class NailCardSO : HandCardSO
{
    public override bool Transformer => true;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        int value = card.returnUnmodifiedData().cardValues[0];

        value += NumberAmount;

        if (value > 9)
        {
            value -= 10;
        }

        card.SetMainValue(value);

        yield return new WaitForGameEndOfFrame();
    }
}
