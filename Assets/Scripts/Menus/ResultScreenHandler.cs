using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultScreenHandler : MonoBehaviour
{
    public MenuMoverHelper ScreenMoverHelper;

    public TextMeshProUGUI ResultsText;

    public TextMeshProUGUI ButtonText;

    public bool Win;

    public IEnumerator InitResults(bool win)
    {
        gameObject.SetActive(true);

        Win = win;

        if (Win)
        {
            ResultsText.text = "You win!";
            ButtonText.text = "Continue";
        }
        else
        {
            ResultsText.text = "You lose...";
            ButtonText.text = "Return to menu";
        }

        yield return ScreenMoverHelper.MoveScreen(true);


    }

    public void ResultsButtonPress()
    {
        GameManager.currRun.CoroutineRunner.StartCoroutine(PressCoro(Win));
    }

    public IEnumerator PressCoro(bool win)
    {

        if (win)
        {
            yield return ScreenMoverHelper.MoveScreen(false);

            yield return GameManager.currRun.TriggerOnEndRound();
        }
        else
        {
            GameManager.CanPause = false;

            yield return GameManager.instance.BlackoutScreen();
            yield return new WaitForSeconds(0.5f);

            GameManager.instance.MainMenu.gameObject.SetActive(true);

            ScreenMoverHelper.PutScreen(false);

            yield return GameManager.instance.RemoveBlackoutScreen();

            GameManager.currRun.EndRun(); 

            GameManager.CanPause = true;
        }

        gameObject.SetActive(false);
    }
}
