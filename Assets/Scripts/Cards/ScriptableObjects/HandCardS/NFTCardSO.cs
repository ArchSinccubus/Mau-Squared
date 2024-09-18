using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New NFTCardSO")]
public class NFTCardSO : LuckySevenSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        yield return card.owner.ReduceMoney((int)(card.owner.RoundMoney / (float)NumberAmount));
    }

    public override int CalcScore(HandCardDataHandler card)
    {
        if (card.state == HandCardState.InHand)
        {
            return base.CalcScore(card);
        }
        else if (card.state == HandCardState.InPlay)
        {
            return base.CalcScore(card) * 2;
        }
        else
        {
            return 0;
        }

    }

    public override string ReturnDescription()
    {
        return base.ReturnDescription().Replace("x", "");
    }
}
