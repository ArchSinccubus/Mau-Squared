using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New LuckySevenSO")]
public class LuckySevenSO : HandCardSO
{
    public override int CalcScore(HandCardDataHandler card)
    {
        if (GameManager.ViewingCollection)
        {
            return 0;
        }

        int score = GameManager.currRun.runState == GameState.InRound ? card.owner.RoundMoney : GameManager.currRun.baseData.PlayerMoney;
        return Mathf.FloorToInt(score * MultAmount);
    }
}
