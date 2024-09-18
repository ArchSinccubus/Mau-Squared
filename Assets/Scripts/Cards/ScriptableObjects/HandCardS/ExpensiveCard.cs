using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New ExpensiveCard")]
public class ExpensiveCardSO : HandCardSO
{
    public override bool overrideMult => base.overrideMult;

    public override float Mult
    {
        get
        {
            return MultAmount;
        }

    }
}
