using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDeck
{
    public Transform transform { get; }

    public ClickableObject DeckClick { get; }

    public void init(DeckHandler handler);

    public Transform returnLocation();

    public void AddCardToDeck(ICardVisuals newCard);

    public void placeDeck(Transform location);

    public IEnumerator MoveDeck(Vector3 location);

    public void OrganizeDeck();

    public void RemoveCardFromDeck(ICardVisuals card);

    public void DrawCardFromDeck(ICardVisuals card);

    public void RemoveCardsFromDeck(params ICardVisuals[] cards);


}
