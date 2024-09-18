using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Side/New WheelFortuneSO")]
public class WheelFortuneSO : SideCardSO
{
    public override bool Clickable => true;

    public override IEnumerator DoCommand(SideCardDataHandler card)
    {
        int num = GameManager.currRun.RoundRand.NextInt(1, 10);

        yield return card.owner.AddScore(num * ScoreAmount);
    }
}
