using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuHandler : MonoBehaviour
{
    public Button StartB, LoadB;

    public TMP_InputField field;

    public MenuMoverHelper helper;

    // Start is called before the first frame update
    void Awake()
    {
        if (File.Exists(Application.persistentDataPath + "/SavedGame.sav"))
        {
            LoadB.interactable = true;
        }
        else
        {
            LoadB.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartClick()
    {
        GameManager.instance.StartCoroutine(StartGame());
    }

    public void LoadClick()
    {
        GameManager.instance.StartCoroutine(LoadGame());
    }

    public IEnumerator StartGame()
    {
        GameManager.CanPause = false;
        yield return GameManager.instance.BlackoutScreen();
        yield return new WaitForSeconds(GameManager.BLACKOUT_WAIT_TIME);

        if (field.text != "")
        {
            GameManager.instance.StartGame(field.text);
        }
        else
        {
            GameManager.instance.StartGame();
        }

        yield return GameManager.instance.RemoveBlackoutScreen();
        GameManager.CanPause = true;
    }

    public IEnumerator LoadGame()
    {
        GameManager.CanPause = false;
        yield return GameManager.instance.BlackoutScreen();
        yield return new WaitForSeconds(GameManager.BLACKOUT_WAIT_TIME);

        GameManager.LoadGame();

        yield return GameManager.instance.RemoveBlackoutScreen();
        GameManager.CanPause = true;
    }


}
