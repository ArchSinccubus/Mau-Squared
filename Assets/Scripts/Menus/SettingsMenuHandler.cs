using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using System.Linq;
using SOHNE.Accessibility.Colorblindness;
using System;

public class SettingsMenuHandler : MonoBehaviour
{
    public MenuMoverHelper helper;

    public Color enabledMode, DisabledMode;

    public Button Gameplay, Video, Graphics, Audio;

    public GameObject GameplayC, VideoC, GraphicsC, AudioC;

    public TMPro.TMP_Dropdown WindowType, Resolution, ColorBlindTypes;

    public UDictionary<string, SettingsBase> settings;

    Resolution[] resolutions;

    Bus Master, Music, SFX;

    public Colorblindness colorBlindManager;

    [SerializeField]
    private EventReference SFXTextEvent;

    [SerializeField]
    private PauseMenuHandler PauseScreen;

    bool FromPause;

    public void LoadMenu()
    {
        resolutions = ReturnNonDupes(Screen.resolutions);

        Resolution.ClearOptions();

        List<string> reses = new List<string>();
        foreach (var item in resolutions)
        {
            reses.Add(item.width + " x " + item.height);
        }

        Resolution.AddOptions(reses); 
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        initMenu();
    }

    public void initMenu()
    {
        if (Gameplay.onClick.GetPersistentEventCount() == 0)
        {
            Gameplay.onClick.AddListener(() => ShowTab(GameplayC, Gameplay));
        }
        if (Video.onClick.GetPersistentEventCount() == 0)
        {
            Video.onClick.AddListener(() => ShowTab(VideoC, Video));
        }
        if (Graphics.onClick.GetPersistentEventCount() == 0)
        {
            Graphics.onClick.AddListener(() => ShowTab(GraphicsC, Graphics));
        }
        if (Audio.onClick.GetPersistentEventCount() == 0)
        {
            Audio.onClick.AddListener(() => ShowTab(AudioC, Audio));
        }


    }

    public void HideAllTabs()
    {
        GameplayC.gameObject.SetActive(false);
        VideoC.gameObject.SetActive(false);
        GraphicsC.gameObject.SetActive(false);
        AudioC.gameObject.SetActive(false);

        Gameplay.image.color = DisabledMode;
        Video.image.color = DisabledMode;
        Graphics.image.color = DisabledMode;
        Audio.image.color = DisabledMode;
    }

    public void ShowTab(GameObject tab, Button button)
    {
        HideAllTabs();
        tab.SetActive(true);
        button.image.color = enabledMode;
    }

    #region Settings Controls



    #endregion

    public IEnumerator OpenSettingsMenu(bool FromPause)
    {
        this.FromPause = FromPause;

        gameObject.SetActive(true);

        yield return helper.MoveScreen(true);
    }

    public void ExitSettings()
    {
        StartCoroutine(HideScreen());
    }

    public IEnumerator HideScreen()
    {
        yield return helper.MoveScreen(false);

        if (FromPause)
        {
            GameManager.instance.StartCoroutine(PauseScreen.OpenPauseMenu());
        }

        gameObject.SetActive(false);

    }

    public void SaveSettings()
    {
        string Final = "";

        GameManager.instance.SavedSettings.speed = (GameSpeed)settings["Speed"].ReturnData();
        GameManager.instance.SavedSettings.MasterVolume = (float)settings["MasterVolume"].ReturnData();
        GameManager.instance.SavedSettings.MusicVolume = (float)settings["MusicVolume"].ReturnData();
        GameManager.instance.SavedSettings.SFXVolume = (float)settings["SFXVolume"].ReturnData();

        GameManager.instance.SavedSettings.mode = (int)settings["WindowMode"].ReturnData();
        GameManager.instance.SavedSettings.resolution = (int)settings["WindowResolution"].ReturnData();

        GameManager.instance.SavedSettings.showKeywords = (bool)settings["Tips"].ReturnData();

        GameManager.instance.SavedSettings.colorBlindMode = (int)settings["ColorBlindType"].ReturnData();


        Final = JsonUtility.ToJson(GameManager.instance.SavedSettings);

        File.WriteAllText(UnityEngine.Application.persistentDataPath + "/Settings.sav", Final);
    }

    public void LoadSettings(SettingsFormat file)
    {
        settings["Speed"].OnLoad(file.speed);

        settings["MasterVolume"].OnLoad(file.MasterVolume);
        settings["MusicVolume"].OnLoad(file.MusicVolume);
        settings["SFXVolume"].OnLoad(file.SFXVolume);

        settings["WindowMode"].OnLoad(file.mode);
        settings["WindowResolution"].OnLoad(file.resolution);

        settings["ColorBlindType"].OnLoad(file.colorBlindMode);

        settings["Tips"].OnLoad(file.showKeywords);

    }

    public void SetSettings(SettingsFormat file)
    {
        SetResolution(file.resolution);
        SetFullScreen(file.mode);
        SetMasterVolume(file.MasterVolume);
        SetMusicVolume(file.MusicVolume);
        SetSFXVolume(file.SFXVolume);
        SetColorBlindType(file.colorBlindMode);
    }

    #region SetSettings
    public void SetResolution(int index)
    {
        Resolution newRes = resolutions[index];
        Screen.SetResolution(newRes.width, newRes.height, Screen.fullScreen);
    }

    public void SetColorBlindType(int type)
    {
        colorBlindManager.InitChange(type);
    }

    public void SetFullScreen(int index)
    {
        FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;

        switch (index)
        {
            case 0:
                mode = FullScreenMode.Windowed;
                break;
            case 1:
                mode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                mode = FullScreenMode.ExclusiveFullScreen;
                break;
            default:
                break;
        }

        Screen.fullScreenMode = mode;
        Screen.fullScreen = mode == FullScreenMode.FullScreenWindow;
    }

    public void SetMasterVolume(float value)
    {
        Master.setVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        Music.setVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        SFX.setVolume(value);
    }

    public void SetToolTipHints(bool show)
    { 
        
    }
    #endregion

    public Resolution[] ReturnNonDupes(Resolution[] orig)
    { 
        List<Resolution> ret = new List<Resolution>();

        foreach (var item in orig)
        {
            if (!ret.Any(o => o.height == item.height && o.width == item.width))
            {
                ret.Add(item);
            }
        }

        return ret.ToArray();
    }
}
