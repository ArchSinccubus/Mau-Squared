using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New 99CardSO")]
public class _99CardSO : HandCardSO
{
    public override bool overrideScore => true;

    public override int Score
    {
        get { return 99; }

    }
}
