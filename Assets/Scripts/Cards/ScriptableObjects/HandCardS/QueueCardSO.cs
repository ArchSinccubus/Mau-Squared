using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New QueueCardSO")]
public class QueueCardSO : HandCardSO
{
    public override bool overrideColor => false;

    public override IEnumerator OnThisPlaced(HandCardDataHandler card)
    {
        HandCardDataHandler bottomCard = GameManager.Round.Pile[0];

        yield return bottomCard.visuals.Peek();

        GameManager.Round.Pile.Remove(bottomCard);
        GameManager.Round.Pile.Add(bottomCard);

        bottomCard.visuals.transform.SetAsLastSibling();

        yield return bottomCard.visuals.Return();
    }
}
