using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New CardSquaredSO")]
public class CardSquaredSO : HandCardSO
{
    public override float CalcMult(HandCardDataHandler card)
    {
        return card.returnModifiedData().cardValues[0];
    }
}
