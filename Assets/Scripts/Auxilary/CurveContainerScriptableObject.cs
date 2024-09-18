using UnityEngine;

[CreateAssetMenu(fileName = "CurveContainer", menuName = "Mau/New Curve")]
public class CurveContainerScriptableObject : ScriptableObject
{
    public AnimationCurve curve;

    public float returnValue(float time)
    {
        return curve.Evaluate(time);
    }

    public float returnTotalTime()
    {
        return curve[curve.length - 1].time;
    }
}