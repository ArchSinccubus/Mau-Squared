using Assets.Scripts.Auxilary;
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

    public Sprite ShopBackground;
    public UDictionary<string, Sprite> Backgrounds;


    public Sprite GetBackground(string name)
    { 
        return Backgrounds[name];
    }

    public Sprite GetRandomBackground(CustomRandom rand, out string name)
    {
        name = Backgrounds.ElementAt(rand.NextInt(0, Backgrounds.Count)).Key;

        return Backgrounds[name];
    }


}

