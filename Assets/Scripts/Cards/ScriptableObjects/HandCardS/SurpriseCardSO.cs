using Assets.Scripts.Auxilary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New SurpriseCardSO")]
public class SurpriseCardSO : HandCardSO
{
    public override bool overrideColor => true;
    public override bool overrideValue => true;

    public override bool overrideScore => true;

    public override int Score => 0;

    public override bool Transformer => true;

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        var values = Enum.GetValues(typeof(Colors));
        Colors color = (Colors)values.GetValue(GameManager.currRun.RoundRand.NextInt(0, 4));

        int number = GameManager.currRun.RoundRand.NextInt(1, 10);

        card.SetTempMainColor(color);
        card.SetTempValues(new List<int> { number }, true);
        card.SetTempNotPreWild();
        card.SetTempNotPostWild();

        yield return new WaitForGameEndOfFrame();
    }
}
