using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAudioManager : MonoBehaviour
{
    [SerializeField]
    private EventReference CardDrawEvent;

    [SerializeField]
    private EventReference CardSmokeEvent;

    [SerializeField]
    private EventReference CardRecycleEvent;

    [SerializeField]
    private EventReference CardTriggerEvent;

    [SerializeField]
    private EventReference CardCleanEvent;

    [SerializeField]
    private EventReference DeckShuffle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawCard(Vector3 location)
    {
        AudioManager.PlayOneShot(CardDrawEvent, location);
    }

    public void TriggerCard(Vector3 location)
    {
        AudioManager.PlayOneShot(CardTriggerEvent, location);
    }

    public void SmokeCard(Vector3 location)
    {
        AudioManager.PlayOneShot(CardSmokeEvent, location);
    }

    public void RecycleCard(Vector3 location)
    {
        AudioManager.PlayOneShot(CardRecycleEvent, location);
    }

    public void CleanCard(Vector3 location)
    {
        AudioManager.PlayOneShot(CardCleanEvent, location);
    }

    public void ShuffleDeck()
    {
        AudioManager.PlayOneShot(DeckShuffle);
    }
}
