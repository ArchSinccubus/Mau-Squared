using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New HostileTakeoverSO")]
public class HostileTakeoverSO : LuckySevenSO
{
    public override bool overrideValue => true;

    public override bool overrideScore => true;



    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        yield return card.owner.AddMoney(card.owner.RoundMoney);
    }

    public override int CalcScore(HandCardDataHandler card)
    {
        if (GameManager.ViewingCollection)
        {
            return 0;
        }

        int score = GameManager.currRun.runState == GameState.InRound ? card.owner.RoundMoney * 2 : GameManager.currRun.baseData.PlayerMoney * 2;
        return Mathf.FloorToInt(score * ScoreAmount);
    }

}
