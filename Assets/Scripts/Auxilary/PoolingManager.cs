using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager
{
    public static List<ICardVisuals> CardPool;
    public static List<ICardVisuals> SideCardPool;
    public static List<ICardVisuals> ActivePool;

    public static void InitPool()
    {
        if (CardPool == null)
        {
            CardPool = new List<ICardVisuals>();

            for (int i = 0; i < 1000; i++) 
            {
                CardVisualHandler newCard = GameManager.Instantiate<HandCardVisualHandler>(GameManager.instance.cardPrefab);

                newCard.gameObject.SetActive(false);

                CardPool.Add(newCard);
            }
        }

        if (SideCardPool == null)
        {
            SideCardPool = new List<ICardVisuals>();

            for (int i = 0; i < 100; i++)
            {
                SideCardVisualHandler newCard = GameManager.Instantiate<SideCardVisualHandler>(GameManager.instance.sideCardPrefab);

                newCard.gameObject.SetActive(false);

                SideCardPool.Add(newCard);
            }
        }

        ActivePool = new List<ICardVisuals>();  
    }

    public static ICardVisuals DrawFromPool(bool side, bool Col)
    {
        ICardVisuals newCard = null;
        if (!side)
        {
            newCard = CardPool[0];

            CardPool.Remove(newCard);
        }
        else
        {
            newCard = SideCardPool[0];

            SideCardPool.Remove(newCard);
        }

        ActivePool.Add(newCard);

        if (!Col)
        {
            newCard.ActivateCard();
        }
        else
        {
            newCard.ActivateForCollection();
        }

        return newCard;
    }

    public static void ReturnToPool(ICardVisuals card)
    {
        card.DeactivateCard();

        ActivePool.Remove(card);

        if (card is SideCardVisualHandler)
        {
            SideCardPool.Add(card);
        }
        else
        {
            CardPool.Add(card);
        }
    }

    public static void ReturnAllToPool()
    {
        for (int i = ActivePool.Count - 1; i >= 0; i--)
        {
            ReturnToPool(ActivePool[i]);
        }
    }
}
