using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChoiceDataHandler : MonoBehaviour, IGameScreen
{
    public EnemyChoiceVisualHandler visuals;

    public NPCHandler[] OpponentChoices;

    Transform IGameScreen.transform { get => transform; }
    public ScreenMoverHelper MoverHelper { get => visuals.mover; }

    public void InitScreen(params NPCHandler[] opponents)
    {
        GameManager.currRun.runState = GameState.inChoice;

        GameManager.currRun.playerVis.deckLoc.DeckClick.AddDelegate(visuals.OpenDeck);
        foreach (var item in GameManager.currRun.player.SideCards)
        {
            item.visuals.SetDraggable(true);
            (item.visuals as SideCardVisualHandler).OnStartDragEvent += visuals.LockSide;
            (item.visuals as SideCardVisualHandler).OnFinishDragEvent += visuals.UnlockSide;
        }

        visuals.Init(opponents);
        OpponentChoices = opponents;

        foreach (var item in visuals.EnemyPreviews)
        {
            item.SelectButton.onClick.RemoveAllListeners();
            item.SelectButton.onClick.AddListener(() => { SelectOpponent(OpponentChoices[item.num]); });

        }

        GameManager.SaveGame();
    }

    public void InitScreen(SaveFormat save)
    {
        GameManager.currRun.runState = GameState.inChoice;

        GameManager.currRun.playerVis.deckLoc.DeckClick.AddDelegate(visuals.OpenDeck);

        foreach (var item in GameManager.currRun.player.SideCards)
        {
            item.visuals.SetDraggable(true);
            (item.visuals as SideCardVisualHandler).OnStartDragEvent += visuals.LockSide;
            (item.visuals as SideCardVisualHandler).OnFinishDragEvent += visuals.UnlockSide;
        }

        visuals.Init(save.NPCHandlerList);
        OpponentChoices = save.NPCHandlerList;

        foreach (var item in visuals.EnemyPreviews)
        {
            item.SelectButton.onClick.RemoveAllListeners();
            item.SelectButton.onClick.AddListener(() => { SelectOpponent(OpponentChoices[item.num]); });

        }
    }

    public void SelectOpponent(NPCHandler npc)
    {
        Deload();
        GameManager.currRun.CoroutineRunner.StartCoroutine(GameManager.currRun.TriggerOnExitChoice(npc));
    }


    public void Deload()
    {
        GameManager.currRun.playerVis.deckLoc.DeckClick.RemoveDelegate(visuals.OpenDeck);
        foreach (var item in GameManager.currRun.player.SideCards)
        {
            item.visuals.SetDraggable(false);
            (item.visuals as SideCardVisualHandler).OnStartDragEvent -= visuals.LockSide;
            (item.visuals as SideCardVisualHandler).OnFinishDragEvent -= visuals.UnlockSide;
        }

        try
        {
            visuals.Deload();
            foreach (var item in OpponentChoices)
            {
                foreach (var card in item.SideCards)
                {
                    card.Deload();
                }
            }
        }
        catch
        { }

    }

    public void SetScreenActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public IEnumerator MoveScreen(bool show)
    {
        yield return MoverHelper.MoveScreen(show);
    }

    public void PutScreen(bool show)
    {
        MoverHelper.PutScreen(show);
    }
}
