
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New PoolClosedSO")]
public class PoolClosedSO : HandCardSO
{
    public override int CalcScore(HandCardDataHandler card)
    {
        if (GameManager.ViewingCollection)
        {
            return 0;
        }
        return card.owner.Hand.Where(o => o.Smoked).Count() * ScoreAmount;
    }
}
