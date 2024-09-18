using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RunSaveData", menuName = "Mau/Run/NewRunSave")]
public class StartingRunData : ScriptableObject
{
    [SerializeField]
    public PlayerData playerData;

    [SerializeField]
    public ShopData shopData;

    [SerializeField]
    public RarityChanceData rarityChanceData;
}
