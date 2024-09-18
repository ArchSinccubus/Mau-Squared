using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckVisualHandler : MonoBehaviour, IDeck
{
    public List<ICardVisuals> Deck;

    public ClickableObject click;

    public ClickableObject DeckClick { get { return click; } }

    public void init(DeckHandler handler)
    {
        Deck = new List<ICardVisuals>();
    }

    public void AddCardToDeck(ICardVisuals newCard)
    {
        Deck.Add(newCard);

        newCard.DisableCardForPlayer();
    }

    public void RemoveCardFromDeck(ICardVisuals card)
    {
        Deck.Remove(card); 

        PoolingManager.ReturnToPool(card);

        OrganizeDeck();
    }

    public void DrawCardFromDeck(ICardVisuals card)
    {
        Deck.Remove(card);

        OrganizeDeck();
    }

    public void RemoveCardsFromDeck(params ICardVisuals[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            RemoveCardFromDeck(cards[i]);
        }
    }

    public void placeDeck(Transform location)
    {
       OrganizeDeck();
        
    }

    public IEnumerator MoveDeck(Vector3 location)
    {
        Vector3 currPos = transform.position;

        Vector2 dir = ((Vector2)location - (Vector2)currPos).normalized;

        while (Vector2.Distance(transform.position, location) > (Time.deltaTime * GameManager.GetTimeSpeed()))
        {
            transform.position += (Vector3)dir * Time.deltaTime * GameManager.GetTimeSpeed();

            //Debug.Log(Vector2.Distance(currPos, newPos));

            yield return new WaitForGameEndOfFrame();
        }

        transform.position = location;
    }

    public void PutDeck(Vector3 location)
    {
        transform.position = location;
    }

    public void OrganizeDeck()
    {
        foreach (Transform item in transform)
        {
            item.localPosition = transform.up * item.GetSiblingIndex() * 1.5f + transform.right * item.GetSiblingIndex();
        }
    }

    public Transform returnLocation()
    {
        return transform; 
    }
}
