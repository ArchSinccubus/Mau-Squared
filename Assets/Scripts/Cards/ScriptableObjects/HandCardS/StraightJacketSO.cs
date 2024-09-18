using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New StraightJacketSO")]
public class StraightJacketSO : HandCardSO
{

    public override bool overrideValue => true;

    public override bool overrideColor => false;

    public override bool overrideScore => true;

    public override int Score
    {
        get => 160;

    }

    public override bool LockValue => true;
}
