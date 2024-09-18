using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New SharedAccountSO")]
public class SharedAccountSO : HandCardSO
{
    public override int CalcScore(HandCardDataHandler card)
    {
        if (GameManager.ViewingCollection)
        {
            return 0;
        }
        if (GameManager.currRun.runState == GameState.InRound)
        {
            EntityHandler target = GameManager.Round.GetOpponent(card.owner);

            return (card.owner.RoundMoney + target.RoundMoney) * ScoreAmount;
        }
        else
            return 0;
    }
}
