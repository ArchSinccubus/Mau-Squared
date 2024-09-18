using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Mau/Cards/Hand/New AnteUpSO")]
public class AnteUpSO : HandCardSO
{

    public override IEnumerator OnThisPlayed(HandCardDataHandler card)
    {
        GameManager.Round.AnteAmount = (int)Mathf.Round(GameManager.Round.AnteAmount * MultAmount);

        GameManager.Round.visuals.Init(GameManager.Round.AnteAmount);

        yield return new WaitForGameEndOfFrame();
    }
}
