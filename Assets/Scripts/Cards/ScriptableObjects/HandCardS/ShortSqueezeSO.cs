using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New ShortSqueezeSO")]
public class ShortSqueezeSO : HandCardSO
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

            return target.RoundMoney * ScoreAmount;
        }
        else
            return 0;
    }
}
