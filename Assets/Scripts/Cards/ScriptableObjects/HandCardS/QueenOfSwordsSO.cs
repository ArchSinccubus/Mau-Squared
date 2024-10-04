using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New QueenOfSwordsSO")]
public class QueenOfSwordsSO : HandCardSO
{
    public override bool Tarot => true;

    public override int CalcScore(HandCardDataHandler card)
    {
        if (GameManager.ViewingCollection)
        {
            return 0;
        }

        if (GameManager.currRun.runState == GameState.InRound)
        {
            return Mathf.FloorToInt(GameManager.Round.Pile.Count * MultAmount);
        }
        return 0;
    }
}
