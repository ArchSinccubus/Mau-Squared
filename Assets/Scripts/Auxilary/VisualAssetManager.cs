using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Visual Asset Manager", menuName = "Mau/New Visual Asset Manager")]
public class VisualAssetManager : ScriptableObject
{
    public Texture2D TempBackTest;

    public UDictionary<string, Texture2D> CardBacks;

    public UDictionary<string, Texture2D> Cards;


}

