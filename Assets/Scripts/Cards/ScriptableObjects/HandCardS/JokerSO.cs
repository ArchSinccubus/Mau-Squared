using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New JokerSO")]
public class JokerSO : HandCardSO
{
    public override bool overrideValue => true;

    public override bool overrideScore => true;

    public override bool Ignore => true;

    public override int Score
    {
        get => 100;

    }
}
