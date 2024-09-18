using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New SweetMemoriesSO")]
public class SweetMemoriesSO : HandCardSO
{
    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        foreach (var item in card.owner.SideCards)
        {
            if (item.Used)
            {
                yield return item.visuals.Pop();
                item.RefreshCard();
            }
        }
    }
}
