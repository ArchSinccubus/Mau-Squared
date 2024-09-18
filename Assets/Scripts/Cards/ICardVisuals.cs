using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void EventDelegate();
public delegate void ToolTipDelegate();

public interface ICardVisuals
{
    public bool Moving { get; }

    public bool Dragged { get; }

    public bool InstantPlay { get; }

    public bool Pulsing { set; }

    public bool Visible { get; set; }

    public Transform transform { get; }

    public RectTransform rect { get; }

    public EventDelegate OnClickedEvent { get; set; }

    public ToolTipDelegate OnToolTip { get; set; }

    public void SetOrigSize();

    public void ResizeCard(float mult);

    public void ResetSizeCard();

    public void ResetCardSlot();

    public void ActivateCard();

    public void ActivateForCollection();

    public void DeactivateCard();

    public Vector2 GetPos();

    public void SetPos(Vector2 pos);

    public void SetPos(Transform pos);

    public void SetSlotLoc(Transform parent);

    public void init(CardSOBase data, bool highlight, bool side, int CanvasOverride);

    public void UpdateCard(List<Colors> newColor, string newText);

    public void UpdateCard(List<Colors> newColors);

    public void UpdateCard(string newText);

    public void SetDraggable(bool isDraggable);

    public void SetShine(bool show);

    public int GetSiblingIndex();

    public IEnumerator MoveCard(Transform newParent);

    public IEnumerator MoveCard(Transform newParent, string Curve);

    public IEnumerator MoveCard(Transform newParent, Vector3 newPos, string Curve);

    public IEnumerator MoveCard(Vector3 newPos);

    public IEnumerator MoveCard(Transform newParent, Vector3 newPos);

    public void SetCardToMenu(int canvasOverride);

    public void SetCardToHand();

    public void EnableCardForPlayer();

    public void HideCard();

    public void RevealCard();

    public void DisableCardForPlayer();

    public void SetSelect(bool select);

    public void SetSmoke(bool smoke);

    public void SetUsable(bool use);

    public void SetHighlightable(bool highlight);

    public IEnumerator PopText(string text);

    public IEnumerator Peek();

    public IEnumerator Return();

    public IEnumerator Shake();

    public IEnumerator Pop();

    public IEnumerator Wiggle();

    public IEnumerator TriggerShine(bool show);

    public IEnumerator Vanish();

    public IEnumerator Flip(bool reveal);
}
