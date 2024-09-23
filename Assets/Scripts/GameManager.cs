using Assets.Scripts.Auxilary;
using Newtonsoft.Json;
using SickDev.CommandSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;
using Debug = UnityEngine.Debug;


//Global Controller for the game at large. Will handle heavy duty tasks and run things along
public class GameManager : MonoBehaviour
{
    #region Static variables
    public static int STARTING_HAND_BASE = 5;
    public static int STARTING_SIDE_DECK_SIZE = 4;
    public static int STARTING_HAND_SHOP_AMOUNT = 9;
    public static int STARTING_SIDE_SHOP_AMOUNT = 4;
    public static int BASE_CARD_REPLACEMENT_NUM = 4;
    public static int BASE_CARD_REMOVAL_NUM = 2;
    public static int BASE_CARD_ADDITION_NUM = 3;
    public static float CARD_ANIM_BASE_SPEED = 1.0f;
    public static float BASE_MOVE_SPEED = 5.5f;
    public static float BLACKOUT_WAIT_TIME = 0.5f;

    public static int CARD_ID_INCREMENTOR = 0;

    public static int NUMBER_SPIN_FPS = 60;
    public static float NUMBER_SPIN_DURATION = 0.75f;

    public static float RESET_PRESS_TIME = 2f;
    #endregion

    public GlobalSaveFormatSO globalVars;

    #region Game Objects
    public static GameManager instance;

    public static RunManager currRun;

    public EventSystem ES;

    public RoundLogicManager currRound;
    public ShopLogicHandler currShop;
    public EnemyChoiceDataHandler currEnemyChoice;

    public PlayerVisualManager playerVis;

    public AssetManagerSO AssetLibrary;
    public EnemyAssetManagerSO EnemyAssetLibrary;
    public VisualAssetManager VisualAssetLibrary;

    public DeckSO testDeck;

    public CardDetailHandler tooltip;

    public Transform OutOfSightLoc;

    public Canvas canvas;
    public Canvas MenuCanvas;

    float resetTimer;
    bool startResetTimer;
    #endregion

    #region RunData
    public SettingsFormat SavedSettings;

    public GameState state;

    public static bool Pause;
    public static bool CanPause;
    public static bool ViewingCollection;

    public int Seed;

    public static RoundLogicManager Round => instance.currRound;
    #endregion

    #region Menu Objects

    public StartMenuHandler StartMenu;
    public MainMenuHandler MainMenu;
    public PauseMenuHandler PauseMenu;
    public SettingsMenuHandler SettingsMenu;
    public ResultScreenHandler ResultMenu;
    public CreditsMenuHandler CreditsMenu;
    public CollectionsMenuHandler CollectionsMenu;

    public GameObject MenuHider;

    public BlackoutHandler blackout;

    #endregion

    #region Prefab References

    public HandCardVisualHandler cardPrefab;
    public SideCardVisualHandler sideCardPrefab;

    #endregion


    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(ES);

        AssetLibrary.Init();
        PoolingManager.InitPool();

        try
        {
            //var test = new GlobalSaveFormat(globalVars);
            var test = File.ReadAllText(Application.persistentDataPath + "/GlobalSave.sav");

            GlobalSaveFormat.instance = JsonConvert.DeserializeObject<GlobalSaveFormat>(test, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });

            
        }
        catch (Exception e)
        {
            Debug.LogError(e);

            var test = new GlobalSaveFormat(globalVars);

            string text = JsonConvert.SerializeObject(test, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });

            File.WriteAllText(Application.persistentDataPath + "/GlobalSave.sav", text);
            //Set stuff up, this is the first time the player boots their game!
        }

        try
        {
            SavedSettings = JsonUtility.FromJson<SettingsFormat>(File.ReadAllText(Application.persistentDataPath + "/Settings.sav"));
        }
        catch (Exception)
        {

            SavedSettings = new SettingsFormat() { MasterVolume = 1, MusicVolume = 1, SFXVolume = 1, speed = GameSpeed.Regular, showKeywords = true, resolution = 1, mode = 1};

            File.WriteAllText(UnityEngine.Application.persistentDataPath + "/Settings.sav", JsonUtility.ToJson(SavedSettings));
        }

        CanPause = true;
        SettingsMenu.LoadMenu();
        SettingsMenu.SetSettings(SavedSettings);
        SettingsMenu.LoadSettings(SavedSettings);

        //Application.logMessageReceived += Application_logMessageReceived;
    }

    public void PauseGame()
    {
        //Time.timeScale = 0f;
        MenuHider.SetActive(true);
        StartCoroutine(PauseMenu.OpenPauseMenu());
        Pause = true;
    }

    public void UnpauseGame()
    {
        //Time.timeScale = 1.0f;
        MenuHider.SetActive(false);
        SettingsMenu.helper.PutScreen(false);
        PauseMenu.helper.PutScreen(false);
        SettingsMenu.gameObject.SetActive(false);
        PauseMenu.gameObject.SetActive(false);
        Pause = false;
    }

    //// Start is called before the first frame update
    //void Start()
    //{
    //    
    //    //StartCoroutine(currRun.StartRound(currRun.generateOpponent()));
    //    //StartCoroutine(currRun.StartShop());
    //}

    //// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P) && CanPause)
        {
            Debug.Log("Escape pressed!");
            if (!Pause)
            {
                PauseGame();

            }
            else
            {
                UnpauseGame();
 
            }

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            startResetTimer = true;

        }

        if (Input.GetKeyUp(KeyCode.R) && startResetTimer)
        {
            startResetTimer = false;
            resetTimer = 0;
        }

        if (startResetTimer)
        {
            resetTimer += Time.deltaTime;
            if (resetTimer >= RESET_PRESS_TIME)
            {
                //reset game here

                startResetTimer = false;
                resetTimer = 0;
            }
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Debug.Log(Input.mousePosition);
        //    Debug.Log(mousePosition);
        //}
        //else
        //{
        //    Time.timeScale = 1;
        //}
    }

    #region Game Handling Methods

    public void StartBurronClick()
    { 
        
    }

    public void StartGame()
    {
        UnpauseGame();
        MainMenu.gameObject.SetActive(false);
        StartMenu.helper.PutScreen(false);
        StartMenu.gameObject.SetActive(false);

        string SeedString = GenerateRandomSeed();

        ObserverManagerSystem.InitLibrary();

        currRun = new RunManager(currRound, currShop, currEnemyChoice);

        currRun.StartRun(AssetLibrary.FetchDeck("Basic"), false, SeedString);

        NPCHandler npc = currRun.generateOpponent();

        StartCoroutine(currRun.StartRound(npc));
    }

    public void StartGame(string Seed)
    {
        UnpauseGame();
        MainMenu.gameObject.SetActive(false);
        StartMenu.helper.PutScreen(false);
        StartMenu.gameObject.SetActive(false);

        Debug.Log("Random Seed is set! Don't forget to disable it to enable true randomness again.");
        //90Q00USZ check this seed for an error with cards not having owners when you drag the side card in the enemy select screen

        currRun = new RunManager(currRound, currShop, currEnemyChoice);

        currRun.MauCardE = 0;
        currRun.MauCardP = 0;
        currRun.MauPunishP = false;
        currRun.MauPunishE = false;

        ObserverManagerSystem.InitLibrary();

        currRun.StartRun(AssetLibrary.FetchDeck("Basic"), false, Seed);

        NPCHandler npc = currRun.generateOpponent();

        StartCoroutine(currRun.StartRound(npc));
    }

    public void StartGame(SaveFormat save)
    {
        UnpauseGame();
        MainMenu.gameObject.SetActive(false);
        StartMenu.helper.PutScreen(false);
        StartMenu.gameObject.SetActive(false);

        Debug.Log("Loading seed from safe file to make sure it's the same!");

        ObserverManagerSystem.InitLibrary();

        currRun = new RunManager(currRound, currShop, currEnemyChoice);
        currRun.LoadRun(save);

        currRun.MauCardE = save.MauCardE;
        currRun.MauCardP = save.MauCardP;
        currRun.MauPunishP = save.MauPunishP;
        currRun.MauPunishE = save.MauPunishE;

        switch (save.CurrState)
        {
            case GameState.InRound:
                currRun.LoadRound(save);
                break;
            case GameState.InShop:
                currRun.LoadShop(save);
                break;
            case GameState.inChoice:
                currRun.LoadChoice(save);
                break;
            default:
                break;
        }
    }

    public void ResumeGame()
    {
        UnpauseGame();
        MenuHider.SetActive(false);
        PauseMenu.gameObject.SetActive(false);
    }

    public void ShowSettings(bool FromPause)
    {
        StartCoroutine(SettingsMenu.OpenSettingsMenu(FromPause));
    }

    public void ShowCredits()
    { 
        StartCoroutine(CreditsMenu.OpenCreditsMenu());
    }

    public void ShowCollections()
    {
        StartCoroutine(CollectionsMenu.OpenCollectionsMenu());
    }

    public void ReturnToMenu()
    {
        currRound.transform.parent.gameObject.SetActive(false);
        currShop.transform.parent.gameObject.SetActive(false);
        currEnemyChoice.transform.parent.gameObject.SetActive(false);

        currRun.EndRun();

        state = GameState.InMenu;

        MainMenu.gameObject.SetActive(true);
        ResumeGame();
    }

    public IEnumerator BlackoutScreen()
    {
        yield return blackout.FadeScreen(true);
    }

    public IEnumerator RemoveBlackoutScreen()
    {
        yield return blackout.FadeScreen(false);
    }

    public static void SaveGame()
    {
        SaveFormat save = new SaveFormat();

        save.IsCustomSeed = currRun.CustomSeed;

        save.CurrState = currRun.runState;
        save.PlayerData = currRun.baseData;
        save.ShopData = currRun.shopData;
        save.RarityChanceData = currRun.rarityData;

        save.currDeck = currRun.player.currDeck.DeckBase;
        save.currSide = currRun.player.SideCards;
        save.RunSeed = currRun.RunSeed;

        save.player = currRun.player;

        save.MauPunishP = currRun.MauPunishP;
        save.MauPunishE = currRun.MauPunishE;
        save.MauCardP = currRun.MauCardP;
        save.MauCardE = currRun.MauCardE;

        save.RoundSeed = currRun.RoundRand.Seed;
        save.RoundCount = currRun.RoundRand.Counter;
        save.ShopSeed = currRun.ShopRand.Seed;
        save.ShopCount = currRun.ShopRand.Counter;

        if (currRun.shopScene.LockedHandCards != null)
        {
            if (currRun.shopScene.LockedHandCards.Count > 0)
            {
                save.LockedHandCards = currRun.shopScene.LockedHandCards;
            }
        }
        if (currRun.shopScene.LockedSideCards != null)
        {
            if (currRun.shopScene.LockedSideCards.Count > 0)
            {
                save.LockedSideCards = currRun.shopScene.LockedSideCards;
            }
        }

        switch (currRun.runState)
        {
            case GameState.InMenu:
                break;
            case GameState.InRound:
                NPCHandler enemy = currRun.roundScene.getNPC();

                save.TurnNum = currRun.roundScene.TurnCount;
                save.RoundNum = currRun.baseData.RoundNum;

                save.PileData = new List<CardPileStruct>();
                foreach (var item in currRun.roundScene.Pile)
                {
                    save.PileData.Add(new CardPileStruct() { card = item, position = item.visuals.GetPos(), rotation = item.visuals.transform.rotation, OwnerPlayer = item.owner.IsPlayer });
                }

                save.RoundAnte = currRun.roundScene.AnteAmount;
                save.MaxPlayerCardScore = currRun.roundScene.MaxPlayerCardScore;
                save.Smoked = currRun.roundScene.TimesSmoked;
                save.Cleaned = currRun.roundScene.TimesCleaned;
                save.Recycled = currRun.roundScene.TimesRecycled;
                save.NPC = enemy;
                break;
            case GameState.InShop:
                save.HandCards = currRun.shopScene.handCards;
                save.SideCards = currRun.shopScene.sideCards;

                save.RefreshCost = currRun.shopScene.RefreshCost;
                save.RemoveCost = currRun.shopScene.RemoveCost;
                save.SideCardCost = currRun.shopScene.SideCardCost;
                break;
            case GameState.inChoice:
                save.NPCHandlerList = currRun.enemyChoiceScene.OpponentChoices;
                break;
            default:
                break;
        }

        string Final = JsonConvert.SerializeObject(save, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });

        File.WriteAllText(Application.persistentDataPath + "/SavedGame.sav", Final);
        //SaveGlobal();

    }

    public static void SaveGlobal()
    {
        string Global = JsonConvert.SerializeObject(GlobalSaveFormat.instance, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });

        File.WriteAllText(Application.persistentDataPath + "/GlobalSave.sav", Global);
    }

    public static void LoadGame()
    {
        string Initial = File.ReadAllText(Application.persistentDataPath + "/SavedGame.sav");

        SaveFormat load = JsonConvert.DeserializeObject<SaveFormat>(Initial, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });

        instance.StartGame(load);
    }

    #endregion

    #region Aux Methods

    public static HandCardDataHandler ReturnBasicCard(EntityHandler entity)
    {
        DeckSO temp = instance.AssetLibrary.FetchDeck("Basic");

        HandCardSO card = temp.DeckBase[currRun.ShopRand.NextInt(0, temp.DeckBase.Count)];

        HandCardDataHandler newCard = new HandCardDataHandler(card, entity, false);

        newCard.InitForRound(entity.IsPlayer);

        return newCard;

    }

    public static Color enumToColor(Colors color)
    {
        switch (color)
        {
            case Colors.Blue: return new Color32(0x33, 0x4D, 0xAC, 0xFF);
            case Colors.Orange: return new Color32(0xFF, 0x87, 0x23, 0xFF);
            case Colors.Red: return new Color32(0xB2, 0x18, 0x18, 0xFF);
            case Colors.Green: return new Color32(0x47, 0xA7, 0x38, 0xFF);
        }
        return Color.white;
    }

    public static float GetTimeSpeed()
    {
        float mult = 1;
        switch (instance.SavedSettings.speed)
        {
            case GameSpeed.Slow:
                mult = 0.5f;
                break;
            case GameSpeed.Regular:
                mult = 1f;
                break;
            case GameSpeed.Fast:
                mult = 1.5f;
                break;
            case GameSpeed.VeryFast:
                mult = 2f;
                break;
            default:
                break;
        }

        return mult * BASE_MOVE_SPEED;
    }

    public static float GetTimeSpeedModifier()
    {
        float mult = 1;
        switch (instance.SavedSettings.speed)
        {
            case GameSpeed.Slow:
                mult = 0.5f;
                break;
            case GameSpeed.Regular:
                mult = 1f;
                break;
            case GameSpeed.Fast:
                mult = 1.5f;
                break;
            case GameSpeed.VeryFast:
                mult = 2f;
                break;
            default:
                break;
        }

        return mult;
    }

    public static Vector2 GetTopOfScreen(Vector3 orig)
    {
        Vector3 convert = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));

        return new Vector3(orig.x, convert.y, orig.z);
    }

    public static CardRarity getRarity(RarityChanceData data)
    {
        float total = data.CommonWeight + data.UncommonWeight + data.RareWeight + data.MythicWeight;

        float roll = currRun.ShopRand.NextFloat(0, total);

        CardRarity rarity = CardRarity.Common;

        while (roll > 0 && rarity != CardRarity.Mythic)
        {
            float temp = getRarityWeight(data, (int)rarity);

            roll -= temp;

            if (roll > 0)
            {
                rarity = (CardRarity)((int)rarity + 1);
            }
        }

        return rarity;
    }

    public static float getRarityWeight(RarityChanceData data, int rarity)
    {
        CardRarity weight = (CardRarity)rarity;

        switch (weight)
        {
            case CardRarity.Common:
                return data.CommonWeight;
            case CardRarity.Uncommon:
                return data.UncommonWeight;
            case CardRarity.Rare:
                return data.RareWeight;
            case CardRarity.Legendary:
                return data.MythicWeight;
            default:
                return 1;
        }
    }

    //This is a stub! In the future, this will be used for higher difficulty;
    public static int getDifficultyModifier()
    {
        return 0;
    }

    public void GenerateToopTip(string Name, string Description, string Score, string Base, string Mult, bool TempBase, bool TempMult)
    {
        tooltip.init(Name, Description, Score, Base, Mult, TempBase, TempMult);
    }

    public void RemoveToolTip()
    {
        tooltip.Deload();
    }

    public static void DisableDrag()
    {
        instance.ES.pixelDragThreshold = int.MaxValue;
    }

    public static void EnableDrag()
    {
        if (!ViewingCollection)
        {
            instance.ES.pixelDragThreshold = currRun.player.CanAct ? 0 : int.MaxValue;
        }
        else
        {
            instance.ES.pixelDragThreshold = 0;
        }
    }

    public static Int64 returnID()
    {
        return Int64.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + CARD_ID_INCREMENTOR++;
    }

    public static string GenerateRandomSeed()
    {
        string Final = "";

        while (Final.Length != 8)
        {
            char c = (char)UnityEngine.Random.Range(33, 127);

            Final += c;
        }

        return Final;
    }

    public static ICardVisuals[] getCardVisuals(params BaseCardDataHandler[] cards)
    {
        ICardVisuals[] cardVisuals = new ICardVisuals[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            cardVisuals[i] = cards[i].visuals;
        }

        return cardVisuals;
    }
    #endregion

    #region Debug Commands

    [Command]
    public static void AddCardToEntity(string name, bool player)
    {
        EntityHandler target = player ? currRun.player : Round.GetOpponent(currRun.player);

        HandCardSO newCard = instance.AssetLibrary.FetchHandCard(name);

        if (newCard != null)
        {
            HandCardDataHandler card = new HandCardDataHandler(newCard, target, false);

            card.InitForRound(player);


            instance.StartCoroutine(target.AddHandCard(card));
        }
        else
            Debug.LogError("No card with this name found!");

    }

    [Command]
    public static void AddPieces()
    {
        EntityHandler target = currRun.player;

        List<HandCardSO> list = new List<HandCardSO>();

        HandCardSO newCard = instance.AssetLibrary.FetchHandCard("Left Crocks of the Prohibited One");
        HandCardSO newCard2 = instance.AssetLibrary.FetchHandCard("Left Furry Glove of the Prohibited One");
        HandCardSO newCard3 = instance.AssetLibrary.FetchHandCard("Right Gottem of the Prohibited One");
        HandCardSO newCard4 = instance.AssetLibrary.FetchHandCard("Right Socks with Sandles of the Prohibited One");

        list.Add(newCard);
        list.Add(newCard2);
        list.Add(newCard3);
        list.Add(newCard4);

        foreach (var item in list)
        {
            HandCardDataHandler card = new HandCardDataHandler(item, target, false);

            card.InitForRound(true);
            target.visuals.HandLoc.SetupSlots(card.visuals);

            instance.StartCoroutine(target.AddHandCard(card));
        }
    }

    [Command]
    public static void AddSideCardToEntity(string name, bool player)
    {
        EntityHandler target = player ? currRun.player : Round.GetOpponent(currRun.player);

        SideCardSO newCard = instance.AssetLibrary.FetchSideCard(name);

        if (newCard != null)
        {
            SideCardDataHandler card = new SideCardDataHandler(newCard, target, false);
            //card.data.OnPickup(card);

            card.InitForRound(player);
            //target.visuals.SideCardLoc.SetupSlots(card.visuals);

            //target.AddSideCard(card);
            target.visuals.PutToSideCards(card.visuals);

        }
        else
            Debug.LogError("No card with this name found!");

    }

    [Command]
    public static void WinRound()
    {
        Round.Winner = currRun.player;
    }

    [Command]
    public static void LoseRound()
    {
        instance.StartCoroutine(Round.GetOpponent(currRun.player).AddScore(100000));
        Round.Winner = Round.GetOpponent(currRun.player);
    }

    [Command]
    public static void DrawCards(int amount, bool player)
    {
        EntityHandler target = player ? currRun.player : Round.GetOpponent(currRun.player);

        instance.StartCoroutine(target.Draw(amount));
    }


    [Command]
    public static void AddPlayerMoney(int amount)
    {
        EntityHandler target = currRun.player;

        if (currRun.runState == GameState.InRound)
        {
            instance.StartCoroutine(target.AddMoney(amount));
        }
        else
        {
            instance.StartCoroutine(currRun.player.AddMoney(amount));
        }
    }

    [Command]
    public static void AddOpponentMoney(int amount) 
    {
        instance.StartCoroutine(Round.GetOpponent(currRun.player).AddMoney(amount));
    }

    [Command]
    public static void ResetShop()
    {
        instance.StartCoroutine(instance.currShop.RefreshShop());
    }

    [Command]
    public static void RevealOpHand()
    {
        EntityHandler target = Round.GetOpponent(currRun.player);

        foreach (var item in target.Hand)
        {
            item.RevealCard();
        }
    }

    [Command]
    public static void SmokeCards(int amount)
    {
        instance.StartCoroutine(SmokeCardsCoru(amount));
    }

    public static IEnumerator SmokeCardsCoru(int amount)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, currRun.player.MakeHandChoice("Smoke", amount,CardMenuChoiceMode.Open, null, HandChoiceType.Smoke));
        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return currRun.player.SmokeCards(currRun.player, cd.result);
    }

    [Command]
    public static void CleanCards(int amount)
    {
        instance.StartCoroutine(CleanCardsCoru(amount));
    }

    public static IEnumerator CleanCardsCoru(int amount)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, currRun.player.MakeHandChoice("Clean", amount, CardMenuChoiceMode.Open, null, HandChoiceType.Clean));
        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return currRun.player.CleanCards(cd.result);
    }

    [Command]
    public static void RecycleCards(int amount)
    {
        instance.StartCoroutine(RecycleCardsCoru(amount));
    }

    public static IEnumerator RecycleCardsCoru(int amount)
    {
        CoroutineWithData<HandCardDataHandler[]> cd = new CoroutineWithData<HandCardDataHandler[]>(GameManager.Round, currRun.player.MakeHandChoice("Recycle", amount, CardMenuChoiceMode.Open, null, HandChoiceType.Recycle));
        yield return cd.coroutine;

        while (cd.result == null)
        {
            yield return new WaitForGameEndOfFrame();
        }

        yield return currRun.player.RecycleCards(cd.result);
    }

    [Command]
    public static void AddScore(int amount)
    { 
        instance.StartCoroutine(currRun.player.AddScore(amount));
    }


    [Command]
    public static void ViewOpponentDeck()
    {
        Round.GetOpponent(currRun.player).currDeck.ViewDeck();
    }
    #endregion
}
