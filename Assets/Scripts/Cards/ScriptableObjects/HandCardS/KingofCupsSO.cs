
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New KingofCupsSO")]
public class KingofCupsSO : HandCardSO
{
    public override bool Tarot => true;

    public override int CalcScore(HandCardDataHandler card)
    {
        if (GameManager.ViewingCollection)
        {
            return 0;
        }
        return card.owner.Hand.Where(o => o.Smoked).Count() * ScoreAmount;
    }
}
