using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsScreenSubMenu : MonoBehaviour
{
    public TextMeshProUGUI SmokeText, RecycleText, CleanText, HighScore, HighCard, HighMoney, FavoriteColor, FavoriteStrat;

    public CardContainer FaveHand, FaveSide;

    public void LoadMenu()
    {
        GameManager.ViewingCollection = true;
        SmokeText.text = GlobalSaveFormat.instance.TimesSmoked.ToString();
        RecycleText.text = GlobalSaveFormat.instance.TimesRecycled.ToString();
        CleanText.text = GlobalSaveFormat.instance.TimesCleaned.ToString();
    
        HighScore.text = GlobalSaveFormat.instance.HighestRoundScore.ToString();
        HighCard.text = GlobalSaveFormat.instance.HighestSingleCardScore.ToString();
        HighMoney.text = GlobalSaveFormat.instance.HighestMoney.ToString();

        FavoriteColor.text = GlobalSaveFormat.instance.FaveColor.ToString();
        FavoriteStrat.text = GlobalSaveFormat.instance.FaveStrat.ToString();

        FaveHand.Init(UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount | UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount, 1, false);
        FaveSide.Init(UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount | UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount, 1, false);

        if (GlobalSaveFormat.GetFavoriteHandCard() != "")
        {
            HandCardSO tempSO = GameManager.instance.AssetLibrary.FetchHandCard(GlobalSaveFormat.instance.FaveHandCard);
            HandCardDataHandler FavHandCard = new HandCardDataHandler(tempSO);
            FavHandCard.InitForView(false, 10000, false);
            FavHandCard.visuals.ResizeCard(1.5f);
            FavHandCard.visuals.SetDraggable(true);
            FaveHand.SetupSlots(FavHandCard.visuals);
            FaveHand.PutNewCard(FavHandCard.visuals);
            FavHandCard.visuals.RevealCard();
        }

        if (GlobalSaveFormat.GetFavoriteSideCard() != "")
        {
            SideCardSO tempSO2 = GameManager.instance.AssetLibrary.FetchSideCard(GlobalSaveFormat.instance.FaveSideCard);
            SideCardDataHandler FavSideCard = new SideCardDataHandler(tempSO2);
            FavSideCard.InitForView(false, 10000, false);
            FavSideCard.visuals.SetDraggable(true);
            FavSideCard.visuals.ResizeCard(1.5f);
            FaveSide.SetupSlots(FavSideCard.visuals);
            FaveSide.PutNewCard(FavSideCard.visuals);
            FavSideCard.visuals.RevealCard();
        }

    }

    public void DeloadMenu()
    {
        GameManager.ViewingCollection = false;
        FaveHand.DestroyCards();
        FaveSide.DestroyCards();    
    }
}
