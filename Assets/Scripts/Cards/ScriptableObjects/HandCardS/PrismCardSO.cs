using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New PrismCard")]
public class PrismCardSO : HandCardSO
{
    public override bool overrideColor => true;
}
