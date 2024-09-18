using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class CanvasScaleFitter : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    CanvasScaler scaler;

    [SerializeField]
    float OrigScale;

    [SerializeField]
    float NatScale;

    public static float CurrScale;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float W = Screen.width;
        float H = Screen.height;

        //float diff = W / H;

        float diff = ((W / H) < NatScale) ? (W / H) : NatScale;
        CurrScale = diff;
        //Debug.Log(diff);

        canvas.transform.localScale = new Vector3(diff, diff, diff) * OrigScale / NatScale;
    }
}
