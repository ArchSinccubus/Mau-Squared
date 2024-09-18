using Assets.Scripts.Auxilary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class HandCardVisualHandler : CardVisualHandler
{
    public bool instantPlay;

    [SerializeField]
    private TextMeshProUGUI Text1, Text2;

    [SerializeField]
    private Image Edge1, Edge2;

    float ShineAmount = 0.25f;


    public override bool InstantPlay => instantPlay;

    public override void UpdateCard(List<Colors> newColor, string newText)
    {
        UpdateCard(newColor);

        UpdateCard(newText);
    }

    public override void UpdateCard(List<Colors> newColors)
    {
        if (!newColors.Contains(Colors.None))
        {
            //texture.color = GameManager.enumToColor(newColor[0]);

            GradientColorKey[] colors = new GradientColorKey[newColors.Count];
            GradientAlphaKey[] alphas = new GradientAlphaKey[newColors.Count];

            for (int i = 0; i < newColors.Count; i++)
            {
                colors[i] = new GradientColorKey(GameManager.enumToColor(newColors[i]), (float)i / newColors.Count);
                alphas[i] = new GradientAlphaKey(1, (float)i / newColors.Count);
            }


            Gradient grad = new Gradient();
            grad.SetKeys(colors, alphas);

            Texture2D temp = GenerateGradientTexture(grad);

            mat.SetTexture("_Gradient", temp);

            texture.material = mat;
        }
    }

    public override void UpdateCard(string newText)
    {
        if (newText != null)
        {
            Text1.SetText(newText);
            Text2.SetText(newText);
        }
    }


    public override void HideCard()
    {
        base.HideCard();

        Edge1.gameObject.SetActive(false);
        Edge2.gameObject.SetActive(false);
        Text1.gameObject.SetActive(false);
        Text2.gameObject.SetActive(false);
    }

    public override void RevealCard()
    {
        base.RevealCard();

        Edge1.gameObject.SetActive(true);
        Edge2.gameObject.SetActive(true);
        Text1.gameObject.SetActive(true);
        Text2.gameObject.SetActive(true);
    }

    public override void SetShine(bool show)
    {
        mat.SetFloat("_ShineAmount", show ? ShineAmount : 0);

        texture.material = mat;
    }

    public override IEnumerator TriggerShine(bool show)
    {
        float finalAmount = show ? ShineAmount : 0;

        float time = 0;

        while (time < 1)
        {
            mat.SetFloat("_ShineAmount", Mathf.Lerp(0, finalAmount, time));
            time += Time.deltaTime * GameManager.GetTimeSpeed();
            yield return new WaitForGameEndOfFrame();
        }

        mat.SetFloat("_ShineAmount", finalAmount);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (Draggable)
        {
            ResetSizeCard();

            if (Interactable && GameManager.currRun.runState == GameState.InRound &&
                        (Vector2.Distance(transform.position, GameManager.Round.TurnPlayer.visuals.PlayLoc.position) < 0.7f))
            {
                Click(true);
            }
            else
            {
                StartCoroutine(MoveCard(SlotLoc.position));
                GameManager.DisableDrag();
            }
            dragged = false;

            //SlotLoc = null;
        }
    }

    public override void Click(bool instant)
    {
        instantPlay = instant;

        base.Click(instantPlay);
    }
}
