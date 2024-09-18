using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New BasicCardSO")]
public class BasicCardSO : HandCardSO
{
    public override bool overrideColor => true;
    public override bool overrideValue => true;

    public override void initCard(HandCardDataHandler card)
    {
        try
        {
            Colors color = cardColors[0];
            int number = cardValues[0];

            List<Colors> CardColor = new List<Colors>() { color };
            List<int> CardValue = new List<int>() { number };
            int CardScore = CardValue[0] * 10;
            card.SetColors(CardColor);
            card.SetValues(CardValue);
        }
        catch (Exception)
        {

            Debug.LogError(Name + " Caused some issues! Please report this as a bug :)");
            throw;
        }
    }
}
