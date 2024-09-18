using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New Draw2Card")]
public class Draw2CardSO : CardDrawSO
{
    public override bool overrideValue => true;

    public override bool overrideColor => false;

    public override bool overrideScore => true;

    public override int Score => 100;
}
