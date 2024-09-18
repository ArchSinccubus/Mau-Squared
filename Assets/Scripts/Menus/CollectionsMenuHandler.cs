using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionsMenuHandler : MonoBehaviour
{
    public GameObject Menu, Viewer;

    public MenuMoverHelper helper;

    public CardContainer CardViewer;

    public StatsScreenSubMenu StatsScreen;

    public CollectionType currCollectionType;

    public Dictionary<string, int> CurrCollection;

    private int index;

    public int pages;

    public int Index { get => index; set 
        {
            if (value < 1)
            {
                value = pages;
            }
            else if (value > pages)
            {
                value = 1;
            }
            index = value;
            LoadCards();
        
        } 
    }

    public IEnumerator OpenCollectionsMenu()
    {
        gameObject.SetActive(true);
        Menu.SetActive(true);
        yield return helper.MoveScreen(true);
    }

    public void OpenStatsMenu()
    {
        helper.PutScreen(false);
        Menu.SetActive(false);
        StatsScreen.LoadMenu();
        StatsScreen.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(helper.MoveScreen(true));
    }

    public void OpenMainMenu()
    {
        helper.PutScreen(false);
        Menu.SetActive(true);
        Viewer.SetActive(false);
        StatsScreen.gameObject.SetActive(false);
        StatsScreen.DeloadMenu();
        CardViewer.DestroyCards();
        StopAllCoroutines();
        StartCoroutine(helper.MoveScreen(true));
    }

    public void OpenCollectionsMenu(bool Side)
    {
        helper.PutScreen(false);
        Menu.SetActive(false);
        Viewer.SetActive(true);

        if (Side)
        {
            CurrCollection = GlobalSaveFormat.instance.UnlockedSideCards;
            currCollectionType = CollectionType.Side;
            pages = 4;
        }
        else 
        {
            CurrCollection = GlobalSaveFormat.instance.UnlockedHandCards;
            currCollectionType = CollectionType.Hand;
            pages = 5;
        }

        gameObject.SetActive(true);
        CardViewer.Init(UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount, 4, false);

        ShowCards();

        StopAllCoroutines();
        StartCoroutine(helper.MoveScreen(true));
    }

    public void CloseMenu()
    {
        StopAllCoroutines();
        StartCoroutine(CloseMenuCoru());
    }

    private void Start()
    {
        //CardViewer.Init(UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount, 4);
        //
        //ShowCards();
    }

    public void ShowCards()
    {
        GameManager.ViewingCollection = true;
        GameManager.instance.tooltip.transform.SetParent(GameManager.instance.MenuCanvas.transform);
        transform.SetAsLastSibling();
        Index = 1;
    }

    private void LoadCards()
    {
        CardViewer.RemoveAllCards();

        List<ICardVisuals> vis = new List<ICardVisuals>();

        int start = (Index - 1) * 20;
        int end = Index * 20;

        for (int i = start; i < end; i++)
        {
            var data = CurrCollection.ElementAt(i);
            BaseCardDataHandler card = GetCardForView(data.Key);

            card.InitForView(card.baseData.SideCard, 5000, true);

            if (data.Value > -1)
            {
                card.RevealCard();
            }
            else
            {
                card.HideCard();
            }
            card.visuals.SetHighlightable(true);
            card.visuals.SetDraggable(true);

            vis.Add(card.visuals);

        }

        CardViewer.SetupSlots(vis.ToArray());


        foreach (var item in vis)
        {
            CardViewer.PutNewCard(item);
        }
    }

    private BaseCardDataHandler GetCardForView(string data)
    {
        CardSOBase SO = null;
        
        switch (currCollectionType)
        {
            case CollectionType.Hand:
                SO = GameManager.instance.AssetLibrary.FetchHandCard(data);

                return new HandCardDataHandler(SO as HandCardSO);
            case CollectionType.Side:
                SO = GameManager.instance.AssetLibrary.FetchSideCard(data);

                return new SideCardDataHandler(SO as SideCardSO);
            default:
                break;
        }

        BaseCardDataHandler card = new BaseCardDataHandler(SO);
        return card;
    }

    public void MoveForward()
    {
        Index++;
    }

    public void MoveBack()
    {
        Index--;
    }

    public IEnumerator CloseMenuCoru()
    {

        GameManager.ViewingCollection = false;
        GameManager.instance.tooltip.transform.SetParent(GameManager.instance.canvas.transform);
        transform.SetAsLastSibling();

        yield return helper.MoveScreen(false);

        if (CardViewer.slots != null)
        {
            CardViewer.DestroyCards();
        }

        Viewer.SetActive(false);
        Menu.SetActive(false);
        gameObject.SetActive(false);

    }
}
