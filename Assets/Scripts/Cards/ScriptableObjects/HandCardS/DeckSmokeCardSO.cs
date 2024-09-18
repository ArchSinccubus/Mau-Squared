using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New DeckSmokeCardSO")]
public class DeckSmokeCardSO : HandCardSO
{
    public IEnumerator SmokeCards(HandCardDataHandler card, EntityHandler target, params HandCardDataHandler[] cards)
    {
        yield return target.SmokeCards(card.owner, cards);
    }
}
