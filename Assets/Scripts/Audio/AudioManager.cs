using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }



    [SerializeField]
    private StudioEventEmitter MainSong;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public static void PlayOneShot(EventReference sound, Vector3 worldPos)
    { 
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public static void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public static void ShiftToRound()
    {
        instance.MainSong.SetParameter("Round", 1);
        instance.MainSong.SetParameter("Shop", 0);
        instance.MainSong.SetParameter("Menu", 0);
    }

    public static void ShiftToShop()
    {
        instance.MainSong.SetParameter("Round", 0);
        instance.MainSong.SetParameter("Shop", 1);
        instance.MainSong.SetParameter("Menu", 0);
    }

    public static void ShiftToMenu()
    {
        instance.MainSong.SetParameter("Round", 0);
        instance.MainSong.SetParameter("Shop", 0);
        instance.MainSong.SetParameter("Menu", 1);
    }

    public static void ShiftToBoss()
    {
        instance.MainSong.SetParameter("Round", 1);
        instance.MainSong.SetParameter("Shop", 1);
        instance.MainSong.SetParameter("Menu", 1);
    }
}
