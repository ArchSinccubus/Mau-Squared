using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SettingsFormat
{
    #region Gameplay
    public GameSpeed speed;

    public bool showKeywords;
    #endregion

    #region Video

    public int mode;
    public int resolution;

    #endregion

    #region Volume

    public float MasterVolume;
    public float MusicVolume;
    public float SFXVolume;

    #endregion
}
