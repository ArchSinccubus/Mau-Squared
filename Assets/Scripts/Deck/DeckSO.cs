using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Deck", menuName = "Mau/Decks/New Deck")]
public class DeckSO : ScriptableObject
{
    public string Name;

    public List<HandCardSO> DeckBase;

    public void Init()
    { 
    
    }

}
