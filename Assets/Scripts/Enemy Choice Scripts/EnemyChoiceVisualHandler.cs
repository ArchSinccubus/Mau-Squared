using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChoiceVisualHandler : MonoBehaviour
{
    public List<EnemyPreviewVisualHandler> EnemyPreviews;
    public ScreenMoverHelper mover;

    Coroutine movement1, movement2;

    bool HoverDeck, HoverSide;
    bool DeckShow, SideShow;
    bool DeckLock, SideLock;

    public void Init(params NPCHandler[] opponents)
    {
        for (int i = 0; i < EnemyPreviews.Count; i++)
        {
            EnemyPreviews[i].InitVisuals(opponents[i], i);
        }
    }

    public void Deload()
    {
        foreach (var item in EnemyPreviews)
        {
            item.Deload();
        }
    }

    private void Update()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        HoverDeck = Array.Exists(hit, o => o.collider.name == "Deck") || DeckLock;
        HoverSide = Array.Exists(hit, o => o.collider.name == "Side") || SideLock;


        if (!GameManager.currRun.playerVis.DeckMoving || !GameManager.instance.currShop.CanAct)
        {
            movement1 = null;
        }

        if (!GameManager.currRun.playerVis.SideMoving || !GameManager.instance.currShop.CanAct)
        {
            movement2 = null;
        }

        if (GameManager.currRun.player.CanAct)
        {
            if (HoverDeck && !DeckShow)
            {
                if (movement1 != null)
                    StopCoroutine(movement1);
                movement1 = StartCoroutine(GameManager.currRun.playerVis.MoveDeck(true));
                DeckShow = true;
            }
            else if (!HoverDeck && DeckShow)
            {
                if (movement1 != null)
                    StopCoroutine(movement1);
                movement1 = StartCoroutine(GameManager.currRun.playerVis.MoveDeck(false));
                DeckShow = false;
            }

            if (HoverSide && !SideShow)
            {
                if (movement2 != null)
                    StopCoroutine(movement2);
                movement2 = StartCoroutine(GameManager.currRun.playerVis.MoveSide(true));
                SideShow = true;
            }
            else if (!HoverSide && SideShow)
            {
                if (movement2 != null)
                    StopCoroutine(movement2);
                movement2 = StartCoroutine(GameManager.currRun.playerVis.MoveSide(false));
                SideShow = false;
            }
        }
    }

    public void OpenDeck()
    {
        DeckLock = true;
    }

    public void CloseDeck()
    {
        DeckLock = false;
    }

    public void LockSide()
    {
        SideLock = true;
    }

    public void UnlockSide()
    {
        SideLock = false;
    }
}
