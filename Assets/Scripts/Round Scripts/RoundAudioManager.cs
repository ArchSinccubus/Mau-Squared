using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundAudioManager : MonoBehaviour
{
    public List<EntityAudioManager> players;

    [SerializeField]
    private StudioEventEmitter MainSong;

    [SerializeField]
    private EventReference PlayerWinEvent;

    [SerializeField]
    private EventReference PlayerLoseEvent;

    public void PlayerWin()
    {
        AudioManager.PlayOneShot(PlayerWinEvent);
    }

    public void PlayerLose()
    {
        MainSong.SetParameter("Lose", 1);
        AudioManager.PlayOneShot(PlayerLoseEvent);
    }
}
