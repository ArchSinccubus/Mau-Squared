using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System.Xml.Linq;
using System.Reflection;
using Codice.CM.SEIDInfo;
using Microsoft.VisualBasic.FileIO;
using System;

public class PopulationMethods : MonoBehaviour
{
    [MenuItem("Eights/PopulateSideCards")]
    static void PopulateSideCards()
    {
        Sprite frontpic = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Textures/Temp Card.png");
        Sprite backPic = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Textures/Card Back.jpg");

        using (TextFieldParser parser = new TextFieldParser("Assets/Resources/Generator Files/Side Cards Data.csv"))
        {
            parser.TextFieldType = FieldType.Delimited;

            parser.SetDelimiters(",");

            while (!parser.EndOfData) 
            {
                string[] fields = parser.ReadFields();

                Debug.Log(fields[4]);
                if (fields[4] != "SO")
                {
                    CardSOBase newCard = Editor.CreateInstance(fields[4]) as CardSOBase;

                    newCard.Rarity = stringToRarity(fields[2]);
                    newCard.Name = fields[0];
                    //newCard.Description = fields[1];
                    newCard.CardFront = frontpic;
                    //newCard.CardBack = backPic;

                    AssetDatabase.CreateAsset(newCard, "Assets/Resources/Side Cards/" + newCard.Name + ".asset");

                }
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();
            }
        }
    }

    [MenuItem("Eights/PopulateHandCards")]
    static void PopulateHandCards()
    {
        Sprite frontpic = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Textures/Temp Card.png");
        Sprite backPic = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Textures/Card Back.jpg");

        using (TextFieldParser parser = new TextFieldParser("Assets/Resources/Generator Files/Hand Cards Data.csv"))
        {
            parser.TextFieldType = FieldType.Delimited;

            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                Debug.Log(fields[4]);
                if (fields[6] != "SO")
                {
                    HandCardSO newCard = Editor.CreateInstance(fields[6]) as HandCardSO;

                    newCard.Rarity = stringToRarity(fields[5]);
                    newCard.Name = fields[0];
                    //newCard.Description = fields[4];
                    newCard.CardFront = frontpic;
                    //newCard.CardBack = backPic;

                    Colors c = stringToColor(fields[2]);
                    

                    CardValue t = stringToValueType(fields[1]);

                    if (t == CardValue.Numeric)
                    {
                        List<int> nums = fields[1].Split('/').Select(int.Parse).ToList();

                        newCard.cardValues = nums;
                    }


                    AssetDatabase.CreateAsset(newCard, "Assets/Resources/Hand Cards/" + newCard.Name + ".asset");

                }
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();
            }
        }
    }

    static CardRarity stringToRarity(string text)
    { 
        switch (text)
        {
            case "Common": return CardRarity.Common;
            case "Uncommon": return CardRarity.Uncommon;
            case "Rare": return CardRarity.Rare;
            case "Legendary": return CardRarity.Legendary;
        }

        return CardRarity.Mythic;
    }

    static Colors stringToColor(string text)
    {
        switch (text)
        {
            case "Red": return Colors.Red;
            case "Orange": return Colors.Orange;
            case "Blue": return Colors.Blue;
            case "Green": return Colors.Green;
            case "Wild": return Colors.None;
        }

        return Colors.None;
    }

    static CardValue stringToValueType(string text)
    {
        switch (text)
        {
            case "Any": return CardValue.None;
            case "Wild": return CardValue.Wild;
        }

        return CardValue.Numeric;
    }


    [MenuItem("Eights/PopulateBasicCards")]
    static void PopulateBasicCards()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <= 9; j++)
            {
                BasicCardSO NewCard = ScriptableObject.CreateInstance<BasicCardSO>();
                NewCard.cardColors = new List<Colors>() { (Colors)i };
                NewCard.cardValues = new List<int> { j };
                NewCard.Name = j + " of " + NewCard.cardColors[0];
                AssetDatabase.CreateAsset(NewCard, "Assets/Resources/Basic Cards/" + (Colors)i + j + ".asset");
            }
        }

        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
    }

    [MenuItem("Eights/PopulateAssetLibrary")]
    static void PopulateAssetLibrary()
    {
        

        AssetManagerSO lib = AssetDatabase.LoadAssetAtPath<AssetManagerSO>("Assets/Resources/AssetsLib.asset");

        lib.Clean();

        string[] BasicCards = Directory.GetFiles("Assets/Resources/Basic Cards", "*.asset", System.IO.SearchOption.TopDirectoryOnly);
        string[] HandCards = Directory.GetFiles("Assets/Resources/Hand Cards", "*.asset", System.IO.SearchOption.TopDirectoryOnly);
        string[] SideCards = Directory.GetFiles("Assets/Resources/Side Cards", "*.asset", System.IO.SearchOption.TopDirectoryOnly);

        foreach (string file in BasicCards) 
        {
            BasicCardSO card = AssetDatabase.LoadAssetAtPath<BasicCardSO>(file);

            lib.InsertBasicCard(card);
        }

        foreach (string file in HandCards)
        {
            HandCardSO card = AssetDatabase.LoadAssetAtPath<HandCardSO>(file);

            lib.InsertHandCard(card);
        }

        foreach (string file in SideCards)
        {
            SideCardSO card = AssetDatabase.LoadAssetAtPath<SideCardSO>(file);

            lib.InsertSideCard(card);
        }

        EditorUtility.SetDirty(lib);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();

    }

    [MenuItem("Eights/OkayFixYourShit")]
    static void FixNoneCards()
    {
        string[] HandCards = Directory.GetFiles("Assets/Resources/Hand Cards", "*.asset", System.IO.SearchOption.TopDirectoryOnly);

        foreach (string file in HandCards)
        {
            HandCardSO card = AssetDatabase.LoadAssetAtPath(file, typeof(HandCardSO)) as HandCardSO;

            if (card.overrideColor && card.cardColors.Count == 0)
            {
                EditorUtility.SetDirty(card);
                card.cardColors.Add(Colors.None);
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
    }

    [MenuItem("Eights/TextToRef")]
    static void ChangeTextToRef()
    {
        string[] HandCards = Directory.GetFiles("Assets/Resources/Hand Cards", "*.asset", System.IO.SearchOption.TopDirectoryOnly);

        foreach (string file in HandCards)
        {
            HandCardSO card = AssetDatabase.LoadAssetAtPath(file, typeof(HandCardSO)) as HandCardSO;

            if (card.overrideColor && card.cardColors.Count == 0)
            {
                EditorUtility.SetDirty(card);
                card.cardColors.Add(Colors.None);
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
    }

    [MenuItem("Eights/PopThatRandomYo")]
    static void PopulateRandomPool()
    {
        EnemyGenerationPoolSO enemy = AssetDatabase.LoadAssetAtPath("Assets/Resources/Enemies/RandomEnemy.asset", typeof(EnemyGenerationPoolSO)) as EnemyGenerationPoolSO;
        AssetManagerSO lib = AssetDatabase.LoadAssetAtPath<AssetManagerSO>("Assets/Resources/AssetsLib.asset");

        EditorUtility.SetDirty(enemy);

        enemy.possibleSideCards.Clear();
        enemy.possibleHandCards.Clear();

        foreach (var item in lib.FetchHandCards(o => true))
        {
            float weight = 0f;

            switch (item.Rarity)
            {
                case CardRarity.Common:
                    weight = 10;
                    break;
                case CardRarity.Uncommon:
                    weight = 5;
                    break;
                case CardRarity.Rare:
                    weight = 1;
                    break;
                case CardRarity.Legendary:
                    weight = 0;
                    break;
                default:
                    break;
            }

            if (weight > 0)
            {
                enemy.possibleHandCards.Add(new EnemyHandCardData() { limit = (4 - (int)item.Rarity), Weight = weight, Card = item });
            }
        }

        enemy.possibleHandCards = enemy.possibleHandCards.OrderByDescending(o => o.Weight).ToList();

        foreach (var item in lib.FetchSideCards(o => true))
        {
            float weight = 0f;

            switch (item.Rarity)
            {
                case CardRarity.Common:
                    weight = 10;
                    break;
                case CardRarity.Uncommon:
                    weight = 5;
                    break;
                case CardRarity.Rare:
                    weight = 1;
                    break;
                case CardRarity.Legendary:
                    weight = 0;
                    break;
                default:
                    break;
            }

            if (weight > 0)
            {
                enemy.possibleSideCards.Add(new EnemySideCardData() { Weight = weight, Card = item });
            }
        }

        enemy.possibleSideCards = enemy.possibleSideCards.OrderByDescending(o => o.Weight).ToList();

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
    }

    [MenuItem("Eights/PopAllEnemies")]
    static void PopulateEnemyPools()
    {
        AssetManagerSO lib = AssetDatabase.LoadAssetAtPath<AssetManagerSO>("Assets/Resources/AssetsLib.asset");

        foreach (var name in Enum.GetValues(typeof(AIStrategy)))
        {
            EnemyGenerationPoolSO enemy = Editor.CreateInstance("EnemyGenerationPoolSO") as EnemyGenerationPoolSO;

            EditorUtility.SetDirty(enemy);

            enemy.mainStrat = (AIStrategy)name;
            enemy.possibleSideCards = new List<EnemySideCardData>();
            enemy.possibleHandCards = new List<EnemyHandCardData>();

            foreach (var item in lib.FetchHandCards(o => o.AIStrategyType.Contains((AIStrategy)name) || o.AIStrategyType.Contains(AIStrategy.All)))
            {
                float weight = 0f;

                switch (item.Rarity)
                {
                    case CardRarity.Common:
                        weight = 10;
                        break;
                    case CardRarity.Uncommon:
                        weight = 5;
                        break;
                    case CardRarity.Rare:
                        weight = 1f;
                        break;
                    case CardRarity.Legendary:
                        weight = 0.1f;
                        break;
                    default:
                        break;
                }

                if (weight > 0)
                {
                    enemy.possibleHandCards.Add(new EnemyHandCardData() { limit = (4 - (int)item.Rarity), Weight = weight, Card = item });
                }
            }

            enemy.possibleHandCards = enemy.possibleHandCards.OrderByDescending(o => o.Weight).ToList();

            foreach (var item in lib.FetchSideCards(o => o.AIStrategyType.Contains((AIStrategy)name) || o.AIStrategyType.Contains(AIStrategy.All)))
            {
                float weight = 0f;

                switch (item.Rarity)
                {
                    case CardRarity.Common:
                        weight = 10;
                        break;
                    case CardRarity.Uncommon:
                        weight = 5;
                        break;
                    case CardRarity.Rare:
                        weight = 1f;
                        break;
                    case CardRarity.Legendary:
                        weight = 0.1f;
                        break;
                    default:
                        break;
                }

                if (weight > 0)
                {
                    enemy.possibleSideCards.Add(new EnemySideCardData() { Weight = weight, Card = item });
                }
            }

            enemy.possibleSideCards = enemy.possibleSideCards.OrderByDescending(o => o.Weight).ToList();

            AssetDatabase.CreateAsset(enemy, "Assets/Resources/Enemies/" + name.ToString() +"Enemy.asset");
        }

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
    }

    [MenuItem("Eights/FillUpThatBasicSaveYo")]
    static void FillUpThatBasicSaveYo()
    {
        AssetManagerSO lib = AssetDatabase.LoadAssetAtPath<AssetManagerSO>("Assets/Resources/AssetsLib.asset");

        GlobalSaveFormatSO sav = AssetDatabase.LoadAssetAtPath<GlobalSaveFormatSO>("Assets/Resources/Global Save Struct.asset");


        foreach (var item in lib.HandCards)
        {
            sav.UnlockedHandCards.Add(item.Key, -1);
        }

        foreach (var item in lib.SideCards)
        {
            sav.UnlockedSideCards.Add(item.Key, -1);
        }

        foreach (var item in Enum.GetValues(typeof(Colors)))
        {
            sav.ColorCount.Add((Colors)item, 0);
        }

        foreach (var item in Enum.GetValues(typeof(AIStrategy)))
        {
            sav.StratCount.Add((AIStrategy)item, 0);
        }

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
    }
}
