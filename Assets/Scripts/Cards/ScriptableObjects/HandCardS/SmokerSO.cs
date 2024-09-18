using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New SmokerSO")]
public class SmokerSO : DeckSmokePlaySO
{
    public override bool overrideScore => true;

    public override int Score
    {
        get => 10;

    }

}
