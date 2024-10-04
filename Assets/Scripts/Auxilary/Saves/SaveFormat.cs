using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Auxilary
{
    public class SaveFormat
    {
        bool IsNewSave;
        public bool IsCustomSeed;

        #region RunData
        public GameState CurrState;
        public PlayerData PlayerData;
        public ShopData ShopData;
        public RarityChanceData RarityChanceData;
        public List<HandCardDataHandler> currDeck;
        public List<SideCardDataHandler> currSide;
        public List<HandCardDataHandler> LockedHandCards;
        public List<SideCardDataHandler> LockedSideCards;

        public bool MauPunishE, MauPunishP;
        public int MauCardE, MauCardP;

        public int RoundNum;
        public string RunSeed;
        public int RoundSeed;
        public int RoundCount;
        public int ShopSeed;
        public int ShopCount;

        #endregion

        #region RoundData

        public int TurnNum;
        public List<CardPileStruct> PileData;

        public int RoundAnte;

        public int MaxPlayerCardScore;

        public PlayerHandler player;
        public NPCHandler NPC;

        public int Smoked, Recycled, Cleaned;

        public string backgroundName;
        #endregion


        #region ShopData

        public List<HandCardDataHandler> HandCards;
        public List<SideCardDataHandler> SideCards;

        public int RefreshCost;
        public int RemoveCost;
        public int SideCardCost;

        #endregion


        #region ChoiceData

        public NPCHandler[] NPCHandlerList;

        #endregion
    }


}