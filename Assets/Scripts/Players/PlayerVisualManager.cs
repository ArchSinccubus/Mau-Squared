using Assets.Scripts.Auxilary;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerVisualManager : EntityVisualManager
{
    public Transform DeckIn, DeckOut;
    bool DeckVisible; //true is in, false is out;

    public bool DeckMoving;

    public Transform SideIn, SideOut;
    public Transform SideContainer;
    bool SideVisible;

    public bool SideMoving;

    public Transform HandIn, HandOut;
    bool HandVisible; 

    public bool HandMoving;

    public Transform MiscLoc, MiscIn, MiscOut;
    public HintManager HintNote;
    bool MiscVisible; 
    
    public bool MiscMoving;


    public Transform PlayedCardIn, PlayedCardOut;
    bool PlayedVisible;

    public bool PlayedCardMoving;


    public TextMeshProUGUI ButtonText;
    public TextMeshProUGUI TurnText;
    public TextMeshProUGUI SideCardTracker;

    public override void Init()
    {
        base.Init();

        DeckVisible = true;
        SideVisible = true;
        HandVisible = true;
        MiscVisible = true;
        PlayedVisible = true;

        SideCardTracker.text = "0/0";
    }

    public void SetButtonText(bool Act)
    {
        ButtonText.text = Act ? "Play" : "Draw";
    }

    public IEnumerator MoveLayout(bool SetVisible)
    {
        CoroutineWaitForList list = new CoroutineWaitForList();

        StartCoroutine(list.CountCoroutine(MoveDeck(SetVisible)));
        StartCoroutine(list.CountCoroutine(MoveHand(SetVisible)));
        StartCoroutine(list.CountCoroutine(MoveSide(SetVisible)));
        StartCoroutine(list.CountCoroutine(MoveMisc(SetVisible)));
        StartCoroutine(list.CountCoroutine(MovePlayedCard(SetVisible)));

        yield return list;
    }

    public void PutLayout(bool SetVisible)
    {
        deckLoc.PutDeck(SetVisible ? DeckIn.position : DeckOut.position);
        HandLoc.transform.position = SetVisible ? HandIn.position : HandOut.position;
        SideContainer.transform.position = SetVisible ? SideIn.position : SideOut.position;
        MiscLoc.transform.position = SetVisible ? MiscIn.position : MiscOut.position;

    }

    public IEnumerator MoveDeck(bool SetVisible)
    {
        DeckMoving = true;
        yield return deckLoc.MoveDeck(SetVisible ? DeckIn.position : DeckOut.position);
        DeckVisible = SetVisible;
        DeckMoving = false;

    }

    public IEnumerator MoveHand(bool SetVisible)
    {
        HandMoving = true;

        Vector3 currPos = HandLoc.transform.position;

        Vector3 location = SetVisible ? HandIn.position : HandOut.position;

        Vector2 dir = ((Vector2)location - (Vector2)currPos).normalized;

        while (Vector2.Distance(HandLoc.transform.position, location) > (Time.deltaTime * GameManager.GetTimeSpeed()))
        {
            HandLoc.transform.position += (Vector3)dir * Time.deltaTime * GameManager.GetTimeSpeed();

            //Debug.Log(Vector2.Distance(currPos, newPos));

            yield return new WaitForGameEndOfFrame();
        }

        HandLoc.transform.position = location;


        HandVisible = SetVisible;

        HandMoving = false;
    }


    public IEnumerator MoveMisc(bool SetVisible)
    {
        MiscMoving = true;

        Vector3 currPos = MiscLoc.transform.position;

        Vector3 location = SetVisible ? MiscIn.position : MiscOut.position;

        Vector2 dir = ((Vector2)location - (Vector2)currPos).normalized;

        while (Vector2.Distance(MiscLoc.transform.position, location) > (Time.deltaTime * GameManager.GetTimeSpeed()))
        {
            MiscLoc.transform.position += (Vector3)dir * Time.deltaTime * GameManager.GetTimeSpeed();

            //Debug.Log(Vector2.Distance(currPos, newPos));

            yield return new WaitForGameEndOfFrame();
        }

        MiscLoc.transform.position = location;

        MiscVisible = SetVisible;

        MiscMoving = false;
    }

    public IEnumerator MoveSide(bool SetVisible)
    {
        SideMoving = true;

        Vector3 currPos = SideContainer.transform.position;

        Vector3 location = SetVisible ? SideIn.position : SideOut.position;

        Vector2 dir = ((Vector2)location - (Vector2)currPos).normalized;

        while (Vector2.Distance(SideContainer.transform.position, location) > (Time.deltaTime * GameManager.GetTimeSpeed()))
        {
            SideContainer.transform.position += (Vector3)dir * Time.deltaTime * GameManager.GetTimeSpeed();

            //Debug.Log(Vector2.Distance(currPos, newPos));

            yield return new WaitForGameEndOfFrame();
        }

        SideContainer.transform.position = location;


        SideVisible = SetVisible;

        SideMoving = false;
    }

    public IEnumerator MovePlayedCard(bool SetVisible)
    {
        PlayedCardMoving = true;

        Vector3 currPos = PlayLoc.transform.position;

        Vector3 location = SetVisible ? PlayedCardIn.position : PlayedCardOut.position;

        Vector2 dir = ((Vector2)location - (Vector2)currPos).normalized;

        while (Vector2.Distance(PlayLoc.transform.position, location) > (Time.deltaTime * GameManager.GetTimeSpeed()))
        {
            PlayLoc.transform.position += (Vector3)dir * Time.deltaTime * GameManager.GetTimeSpeed();

            //Debug.Log(Vector2.Distance(currPos, newPos));

            yield return new WaitForGameEndOfFrame();
        }

        PlayLoc.transform.position = location;


        PlayedVisible = SetVisible;

        PlayedCardMoving = false;
    }

    public void SetNote(bool trigger)
    {
        HintNote.gameObject.SetActive(trigger);
    }

    public void UpdateSideCardText(int amount, int max)
    {
        SideCardTracker.text = amount + "/" + max;
    }

    public void ToggleTurnText(bool show)
    { 
        TurnText.gameObject.SetActive(show);
    }
}
