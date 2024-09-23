using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDetailHandler : MonoBehaviour
{
    public RectTransform rect;
    public RectTransform child;
    public RectTransform hints;
    public RectTransform hintsChild;

    public TextMeshProUGUI CardTitle;
    public TextMeshProUGUI CardDescription;
    public TextMeshProUGUI CardScore;

    public UDictionary<string, GameObject> HintTextBoxes;

    public void init(string Name, string Description, string Score, string Base, string Mult, bool tempBase, bool TempMult)
    {
        gameObject.SetActive(true);
        CardTitle.text = Name;

        Description = ActivateHintBoxes(Description);

        CardDescription.text = Description;
 
        if (Score != "")
        {
            string deepDive = "";

            string finalBase = tempBase ? "<color=#008c84ff>" + Base + "</color>" : Base;
            string finalMult = TempMult ? "<color=#008c84ff>" + Mult + "</color>" : Mult;

            string finalScore = (tempBase || TempMult) ? "<color=#008c84ff>" + Score + "</color>" : Score;

            if (Mult != "")
            {
                deepDive = " (" + finalBase + " x " + finalMult + ")";
            }

            CardScore.text = finalScore + " Points" + deepDive;
        }
        else
        {
            CardScore.gameObject.SetActive(false);
        }
        
    }

    public void Deload()
    {
        foreach (var item in HintTextBoxes)
        {
            item.Value.SetActive(false);
        }

        CardTitle.text = "";
        CardDescription.text = "";
        CardScore.text = "";
        CardScore.gameObject.SetActive(true);
        gameObject.SetActive(false);     
    }

    public string ActivateHintBoxes(string desc)
    {
        string[] words = desc.Split(" ");

        foreach (var item in words)
        {
            if (HintTextBoxes.ContainsKey(item.ToLower()))
            {
                desc = desc.Replace(item, "<b>" + item + "</b>");
                if (GameManager.instance.SavedSettings.showKeywords)
                {
                    HintTextBoxes[item.ToLower()].SetActive(true);
                }
            }
        }

        return desc;
    }

    private void Update()
    {
        
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;

        bool[] temp = rect.ReturnCornersVisibleFrom(Camera.main);

        bool[] hintTemp = hints.ReturnCornersVisibleFrom(Camera.main);

        Vector2 newPivot = new Vector2(temp[3] ? 0:1, temp[1] ? 0:1);

        child.pivot = newPivot;

        Vector2 newPos = new Vector2(child.rect.width * ((hintTemp[3] ? 1 : -1) + (temp[3] ? 0 : -1)), child.rect.height * (temp[1] ? 1 : 0));

        hintsChild.localPosition = newPos;
        //Debug.Log(temp[1] + " " + temp[3]);
    }
}
