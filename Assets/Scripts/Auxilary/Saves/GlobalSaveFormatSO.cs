using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Global Save Struct", menuName = "Mau/Global Save Struct")]
public class GlobalSaveFormatSO : ScriptableObject
{
    [SerializeField]
    public UDictionary<string, int> UnlockedHandCards;

    [SerializeField]
    public UDictionary<string, int> UnlockedSideCards;

    [SerializeField]
    public UDictionary<AIStrategy, int> StratCount;

    [SerializeField]
    public UDictionary<Colors, int> ColorCount;
}

