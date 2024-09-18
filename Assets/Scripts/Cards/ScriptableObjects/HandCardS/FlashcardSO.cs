using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New FlashcardSO")]
public class FlashcardSO : HandCardSO
{

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        yield return card.owner.Draw(NumberAmount);

        CoroutineWaitForList list = new CoroutineWaitForList();

        for (int i = card.owner.Hand.Count - 1; i >= card.owner.Hand.Count - NumberAmount; i--)
        {
            card.owner.Hand[i].MultTempMultScore(MultAmount);
        }
    }
}
