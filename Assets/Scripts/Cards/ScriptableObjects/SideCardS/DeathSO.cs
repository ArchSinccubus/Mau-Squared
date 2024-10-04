using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New DeathSO")]
public class DeathSO : SideCardSO
{
    public override bool Tarot => true;

    public override bool Copyable => false;

    public IEnumerator RemoveCard(SideCardDataHandler card)
    {
        yield return card.TriggerCardEffect();

        card.owner.RemoveSideCard(card);
    }
}
