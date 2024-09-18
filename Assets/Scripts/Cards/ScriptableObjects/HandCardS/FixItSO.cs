using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New FixItSO")]
public class FixItSO : HandCardSO
{
    public override bool overrideColor => false;

    public override IEnumerator OnThisDrawn(HandCardDataHandler card)
    {
        Colors newColor = Colors.None;

        while (newColor == Colors.None && card.returnModifiedData().cardColors.Count < 4)
        {
            var values = Enum.GetValues(typeof(Colors));
            Colors color = (Colors)values.GetValue(GameManager.currRun.RoundRand.NextInt(0, 4));

            if (!card.returnModifiedData().cardColors.Contains(color))
            {
                newColor = color;
            }
        }

        if (newColor != Colors.None)
        {
            card.SetTempColors(new List<Colors>() { newColor }, false);

            yield return card.visuals.Wiggle();
        }
    }
}
