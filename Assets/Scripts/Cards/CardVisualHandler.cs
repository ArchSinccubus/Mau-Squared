using Assets.Scripts.Auxilary;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CardVisualHandler : MonoBehaviour, ICardVisuals, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    static int CANVAS_RAISE_AMOUNT = 15;

    protected Material mat;

    private Coroutine pulsing;
    public bool Pulsing { set 
        {
            if (value)
            {
                if (pulsing == null)
                {
                    pulsing = StartCoroutine(Pulse());
                }
            }
            else
            {
                StopCoroutine(pulsing);
                transform.localScale = Vector3.one;
                pulsing = null;
            }
        } 
    }

    public virtual bool InstantPlay { get => false; }

    public RectTransform rect { get 
        {
            return rectTrans;    
        } 
    }
    public bool Moving { get => moving; }

    public bool Dragged { get => dragged; }

    public bool Visible { get; set; }

    public EventDelegate OnClickedEvent { get; set; }

    public ToolTipDelegate OnToolTip { get; set; }

    public bool Highlighted;

    [SerializeField]
    protected bool IsHighlightable;

    [SerializeField]
    protected bool Interactable;

    [SerializeField]
    protected bool Draggable;

    public bool ShowBack;

    public float Speed;

    public Texture2D CardFront, CardBack;

    [SerializeField]
    protected Image texture;


    [SerializeField]
    protected Image button;

    [SerializeField]
    TextMeshProUGUI effectText;

    protected Transform SlotLoc;
    public Transform CardLoc, TextLoc;

    public Canvas cardCanvas;

    public RectTransform rectTrans;

    protected bool moving;
    protected bool dragged;
    protected Vector2 origScale;

    bool CanvasOverrideFlag;
    int CanvasOverrideValue;

    public Image Border;


    public void init(CardSOBase data, bool highlight, bool side, int CanvasOverride)
    {
        gameObject.name = data.Name;

        CardFront = data.CardFront.texture;
        CardBack = GameManager.instance.VisualAssetLibrary.TempBackTest;

        IsHighlightable = highlight;

        mat = Instantiate(texture.material);
        mat.SetTexture("_CardBack", GameManager.instance.VisualAssetLibrary.TempBackTest);
        mat.SetTexture("_CardFront", CardFront);

        texture.material = mat;

        Draggable = false;

        HideCard();
        CanvasOverrideFlag = false;
        if (CanvasOverride != 0)
        {
            SetCardToMenu(CanvasOverride);
            CanvasOverrideValue = CanvasOverride;
        }
        
        Border.sprite = GameManager.instance.AssetLibrary.GetBorder(data.Rarity);
        
        origScale = Vector2.zero;
    }

    private void Update()
    {
        CardLoc.localScale = new Vector3(CardLoc.localScale.x, CardLoc.localScale.y, 1);

        if (SlotLoc != null && !moving && !dragged)
        {
            if (Vector2.Distance(transform.position, SlotLoc.position) < 0.1f)
            {
                transform.position = new Vector3(SlotLoc.position.x, SlotLoc.position.y, -SlotLoc.position.y);
                //transform.position = new Vector3(SlotLoc.position.x, SlotLoc.position.y, 0);
            }
            else
            {
                //CardLoc.transform.localScale = Vector3.one;
        
                transform.position = Vector2.Lerp(transform.position, SlotLoc.position, 0.05f * GameManager.GetTimeSpeed());
                //transform.position = Vector2.Lerp(transform.position, SlotLoc.position, 0);
        
            }
        }
        
    }

    public virtual void UpdateCard(List<Colors> newColor, string newText)
    {
        UpdateCard(newColor);

        UpdateCard(newText);
    }

    public virtual void UpdateCard(List<Colors> newColors)
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

    public virtual void UpdateCard(string newText)
    {
       
    }

    public void SetOrigSize()
    {
        origScale = CardLoc.localScale;
    }

    public void ResizeCard(float mult)
    {
        if (origScale == Vector2.zero)
        {
            CardLoc.localScale = new Vector3(rectTrans.localScale.x * mult, rectTrans.localScale.y * mult, 1); 
            SetOrigSize();
        }
        else
        {
            CardLoc.localScale = origScale * mult;
        }

    }

    public void ResetSizeCard()
    {
        if (origScale != Vector2.zero)
        {
            CardLoc.localScale = origScale;
        }
        else
        {
            CardLoc.localScale = Vector3.one;
        }
    }

    public void ResetCardSlot()
    {
        SlotLoc = null;
    }

    public int GetSiblingIndex()
    {
        if (SlotLoc != null)
        {
            return SlotLoc.transform.GetSiblingIndex();
        }
        else
        {
            return transform.parent.GetSiblingIndex();
        }
    }

    public void SetSelect(bool select)
    {
        mat.SetInt("_Selected", select ? 1 : 0);

        if (!select)
        {
            ResetSizeCard();
        }

        Highlighted = select;

        if (select)
        {
            BumpCardToFront();
        }
        else
        {
            BumpCardBack();
        }

        //cardCanvas.overrideSorting = select;
        texture.material = mat;
    }

    public void SetUsable(bool use)
    {
        mat.SetInt("_Usable", use ? 1 : 0);

        texture.material = mat;
    }

    public void SetHighlightable(bool highlight)
    {
        IsHighlightable = highlight;
    }

    public void SetPos(Vector2 pos)
    {
        transform.position = pos;
    }

    public void SetPos(Transform pos)
    {
        transform.SetParent(pos);
        transform.localPosition = Vector2.zero;
    }

    public void SetSlotLoc(Transform parent)
    {
        SlotLoc = parent;
    }

    public void EnableCardForPlayer()
    {
        Interactable = true;
        button.raycastTarget = true;
        SetHighlightable(true);
    }

    public void DisableCardForPlayer()
    {
        Interactable = false;
        button.raycastTarget = false;
        SetHighlightable(false);
        ResetSizeCard();
    }

    public void ActivateCard()
    {
        gameObject.SetActive(true);
        transform.SetParent(GameManager.instance.canvas.transform);
        transform.localScale = Vector3.one;
        transform.position = Vector3.zero; 
        transform.rotation = Quaternion.identity;
    }

    public void ActivateForCollection()
    {
        gameObject.SetActive(true);
        transform.SetParent(GameManager.instance.MenuCanvas.transform);
        transform.localScale = Vector3.one;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    public void DeactivateCard()
    {
        gameObject.SetActive(false);
        transform.SetParent(null);
        transform.position = Vector3.zero;
    }


    public virtual void HideCard()
    {
        texture.color = Color.white;

        mat.SetInt("_Revealed", 0);

        //cardText.enabled = false;

        Visible = false;
        ShowBack = true;
        texture.material = mat;

        Border.gameObject.SetActive(false); 
    }

    public virtual void RevealCard()
    {
        //cardText.enabled = true;

        mat.SetInt("_Revealed", 1);

        Visible = true;
        ShowBack = false;
        texture.material = mat;

        Border.gameObject.SetActive(true);
    }

    public Vector2 GetPos()
    {
        return transform.position;
    }

    public void SetCardToMenu(int canvasOverride)
    {
        CanvasOverrideFlag = true;
        cardCanvas.overrideSorting = true;
        cardCanvas.sortingOrder = canvasOverride + 1;
        CanvasOverrideValue = canvasOverride + 1;

    }

    public void SetCardToHand()
    {
        CanvasOverrideFlag = false;
        cardCanvas.overrideSorting = false;
        cardCanvas.sortingOrder = CANVAS_RAISE_AMOUNT;
        CanvasOverrideValue = 1;
    }

    public void BumpCardToFront()
    {
        cardCanvas.overrideSorting = true;
        cardCanvas.sortingOrder = CanvasOverrideValue + CANVAS_RAISE_AMOUNT;
    }

    public void BumpCardBack()
    {
        cardCanvas.overrideSorting = CanvasOverrideFlag;
        cardCanvas.sortingOrder = CanvasOverrideValue + 1;
    }

    public IEnumerator MoveCard(Vector3 newPos)
    {
        BumpCardToFront();
        Transform parent = transform.parent;
        int index = transform.GetSiblingIndex();
        //transform.SetParent(GameManager.instance.canvas.transform);
        transform.SetAsLastSibling();

        yield return MoveBySpace(newPos);

        transform.SetParent(parent);
        transform.SetSiblingIndex(index);
        BumpCardBack();
    }

    public virtual IEnumerator MoveCard(Transform newParent)
    {
        BumpCardToFront();
        transform.SetParent(GameManager.instance.canvas.transform);
        transform.SetAsLastSibling();

        yield return MoveBySpace(newParent.position);

        transform.SetParent(newParent);
        BumpCardBack();
    }

    public virtual IEnumerator MoveCard(Transform newParent, string Curve)
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves[Curve];
        BumpCardToFront();
        transform.SetParent(GameManager.instance.canvas.transform);
        transform.SetAsLastSibling();

        yield return MoveByCurve(newParent.position, curve);

        transform.SetParent(newParent);
        BumpCardBack();
    }

    public IEnumerator MoveCard(Transform newParent, Vector3 newPos, string Curve)
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves[Curve];
        cardCanvas.overrideSorting = true;
        transform.SetParent(GameManager.instance.canvas.transform);
        transform.SetAsLastSibling();

        yield return MoveByCurve(newPos, curve);

        transform.SetParent(newParent);
        BumpCardBack();
    }

    public IEnumerator MoveCard(Transform newParent, Vector3 newPos)
    {
        transform.SetParent(GameManager.instance.canvas.transform);
        transform.SetAsLastSibling();

        yield return MoveBySpace(newPos);
        transform.SetParent(newParent);
    }

    public IEnumerator MoveBySpace(Vector3 newPos)
    {
        moving = true;

        Vector3 currPos = transform.position;

        Vector2 dir = ((Vector2)newPos - (Vector2)currPos).normalized;

        while (Vector2.Distance(transform.position, newPos) > (Speed * Time.deltaTime * GameManager.GetTimeSpeed()))
        {
            transform.position += (Vector3)dir * Time.deltaTime * Speed * GameManager.GetTimeSpeed();

            //Debug.Log(Vector2.Distance(currPos, newPos));
            

            yield return new WaitForGameEndOfFrame();
        }

        yield return new WaitForGameEndOfFrame();
        transform.position = newPos;
        

        GameManager.EnableDrag();
        moving = false;
    }

    public IEnumerator MoveByTime(Vector3 newPos)
    {
        moving = true;

        Vector3 currPos = transform.position;

        float time = 0;
        
        while (time < GameManager.GetTimeSpeed()) 
        {
            //Just in case I ever forget to set Speed and it's 0 for any reason
            transform.position = Vector3.Lerp(currPos, newPos, time * GameManager.GetTimeSpeed());
            time += Time.deltaTime;
            //Debug.Log(transform.position);
            //Debug.Log(time);
            yield return new WaitForGameEndOfFrame();
        }

        yield return new WaitForGameEndOfFrame();

        transform.position = newPos;

        GameManager.EnableDrag();
        moving = false;
    }

    public IEnumerator MoveByCurve(Vector3 newPos, CurveContainerScriptableObject curve)
    {
        moving = true;
        Vector3 currPos = transform.position;

        float time = 0;

        while (time < curve.returnTotalTime())
        {
            //Just in case I ever forget to set Speed and it's 0 for any reason
            transform.position = Vector3.Lerp(currPos, newPos, curve.returnValue(time * GameManager.GetTimeSpeed()));
            time += Time.deltaTime;
            //Debug.Log(transform.position);
            //Debug.Log(time);
            yield return new WaitForGameEndOfFrame();
        }

        yield return new WaitForGameEndOfFrame();

        transform.position = newPos;

        GameManager.EnableDrag();
        moving = false;
    }


    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Vector3 pos = SlotLoc != null ? SlotLoc.position : transform.parent.position;

        if (Interactable &&
                    (Vector2.Distance(transform.position, pos) < 0.5f))
        {
            Click(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (IsHighlightable && !moving)
        {
            BumpCardToFront();
            ResizeCard(1.25f);
        }

        if (!ShowBack)
        {
            OnToolTip.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        if (IsHighlightable && !Highlighted)
        {
            BumpCardBack();

            ResetSizeCard();
        }

        if (!ShowBack)
        {
            GameManager.instance.RemoveToolTip();
        }
    }

    public virtual void Click(bool instant)
    {
        if (OnClickedEvent != null)
        {
            //instantPlay = instant;
            GameManager.instance.RemoveToolTip();
            OnClickedEvent.Invoke();
        }
    }


    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (Draggable)
        {
            dragged = true;
            cardCanvas.overrideSorting = true;
            //SlotLoc = transform.parent;
            //transform.SetParent(GameManager.instance.canvas.transform);
            transform.SetAsLastSibling();
            //transform.position = Camera.main.ScreenToViewportPoint(eventData.position);
            StopAllCoroutines();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Draggable)
        {
                transform.position = (Vector2)Camera.main.ScreenToWorldPoint(eventData.position);
            
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (Draggable)
        {
            ResetSizeCard();

            StartCoroutine(MoveCard(SlotLoc.position));
            GameManager.DisableDrag();
            dragged = false;
            //SlotLoc = null;
        }
    }

    public void SetDraggable(bool isDraggable)
    {
        Draggable = isDraggable;
    }

    public void SetSmoke(bool smoke)
    {
        mat.SetInt("_Smoked", smoke ? 1 : 0);

        texture.material = mat;
    }

    public virtual void SetShine(bool show)
    {
        
    }

    public virtual IEnumerator PopText(string text)
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["pop"];

        effectText.SetText(text);
        TextLoc.gameObject.SetActive(true);

        float time = 0;

        while (time < curve.returnTotalTime())
        {

            effectText.transform.localScale = Vector3.one * curve.returnValue(time);
            time += Time.deltaTime * 2;
            yield return new WaitForGameEndOfFrame();
        }

        effectText.transform.localScale = Vector3.one;
        yield return new WaitForGameSeconds(0.1f);
        TextLoc.gameObject.SetActive(false);

        effectText.SetText("");

    }

    public IEnumerator Peek()
    {
        yield return MoveByTime(GetPos() + Vector2.left * 1.5f);
    }

    public IEnumerator Return()
    {
        yield return MoveByTime(GetPos() - Vector2.left * 1.5f);
    }

    public IEnumerator Shake()
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["shake"];

        Vector3 basePos = transform.position;

        float time = 0;

        while (time < curve.returnTotalTime())
        {
            float temp = curve.returnValue(time);

            CardLoc.transform.position = new Vector3(temp * Mathf.Abs(temp) + basePos.x, basePos.y, 0);
            time += Time.deltaTime * GameManager.GetTimeSpeedModifier();
            yield return new WaitForGameEndOfFrame();
        }

        CardLoc.transform.position = basePos;
    }

    public IEnumerator Pop()
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["pop"];

        float time = 0;

        while (time < curve.returnTotalTime())
        {

            CardLoc.transform.localScale = origScale * curve.returnValue(time);
            time += Time.deltaTime * GameManager.GetTimeSpeedModifier();
            yield return new WaitForGameEndOfFrame();
        }

        CardLoc.transform.localScale = origScale;
    }

    public IEnumerator ResizePop(float newSize)
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["pop"];

        float time = 0;
        Vector3 currScale = origScale;

        while (time < curve.returnTotalTime())
        {

            CardLoc.transform.localScale = currScale * curve.returnValue(time);
            time += Time.deltaTime * GameManager.GetTimeSpeedModifier();
            currScale = origScale * Mathf.Lerp(origScale.magnitude, newSize, time);
            yield return new WaitForGameEndOfFrame();
        }

        currScale = origScale * newSize;
        CardLoc.transform.localScale = currScale;
    }

    public IEnumerator Wiggle()
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["wiggle"];

        float time = 0;

        while (time < curve.returnTotalTime())
        {
            CardLoc.transform.rotation = Quaternion.Euler(0, 0, curve.returnValue(time) * 50);
            time += Time.deltaTime * GameManager.GetTimeSpeedModifier();
            yield return new WaitForGameEndOfFrame();
        }

        CardLoc.transform.rotation = Quaternion.identity;

    }

    public virtual IEnumerator TriggerShine(bool show)
    {
        yield return new WaitForGameEndOfFrame();
    }

    public IEnumerator Flip(bool reveal)
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["flip"];

        float time = 0;

        while (time < curve.returnTotalTime() / 2)
        {
            CardLoc.transform.rotation = Quaternion.Euler(0, Mathf.Lerp(0, 90, curve.returnValue(time) * 2), 0);
            time += Time.deltaTime * GameManager.GetTimeSpeedModifier();
            yield return new WaitForGameEndOfFrame();
        }

        //time = 1f;
        if (!reveal)
        {
            HideCard();
        }
        else
        {
            RevealCard();
        }

        while (time < curve.returnTotalTime())
        {
            CardLoc.transform.rotation = Quaternion.Euler(0, Mathf.Lerp(0, 90, curve.returnValue((1 - time) * 2)), 0);
            time += Time.deltaTime * GameManager.GetTimeSpeedModifier();
            yield return new WaitForGameEndOfFrame();
        }

        transform.rotation = Quaternion.identity;
    }

    public IEnumerator Pulse()
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["pulse"];
        float time = 0;


        while (true)
        {
            time = 0;

            while (time < curve.returnTotalTime())
            {

                CardLoc.transform.localScale = Vector3.one * curve.returnValue(time);
                time += Time.deltaTime;
                yield return new WaitForGameEndOfFrame();
            }

            CardLoc.transform.localScale = Vector3.one;

            yield return new WaitForGameSeconds(0.5f);
        }    
    }

    public IEnumerator Vanish()
    {
        CurveContainerScriptableObject curve = GameManager.instance.AssetLibrary.curves["flip"];

        float time = 0;

        while (time < curve.returnTotalTime() / 2)
        {
            CardLoc.transform.rotation = Quaternion.Euler(0, Mathf.Lerp(0, 90, curve.returnValue(time * 2)), 0);
            CardLoc.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + time * 2, transform.localScale.z);
            time += Time.deltaTime * GameManager.GetTimeSpeedModifier();
            yield return new WaitForGameEndOfFrame();
        }
    }

    public Texture2D GenerateGradientTexture(Gradient grad)
    {
         float width = 256;
         float height = 64;

        Texture2D _tempTexture = new Texture2D((int)width, (int)height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = grad.Evaluate(0 + (x / width));
                _tempTexture.SetPixel(x, y, color);
            }
        }
        _tempTexture.wrapMode = TextureWrapMode.Clamp;
        _tempTexture.Apply();
        return _tempTexture;
    }
}
