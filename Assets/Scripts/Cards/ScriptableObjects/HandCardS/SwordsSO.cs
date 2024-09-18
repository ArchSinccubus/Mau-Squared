using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New SwordsSO")]
public class SwordsSO : HandCardSO
{
    public override bool overrideMult => true;

    public override int BasePrice => 1;

    public override float Mult
    {
        get => MultAmount;
    }
}
