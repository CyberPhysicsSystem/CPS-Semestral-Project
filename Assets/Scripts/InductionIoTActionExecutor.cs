using UnityEditor.PackageManager.UI;
using UnityEngine;

public class InductionIoTActionExecutor : MonoBehaviour
{
    public Material emmisiveMaterial;
    [ColorUsage(true, true)]
    public Color preHeatColor, postHeatColor;
    [ColorUsage(true, true)]
    public Color fromColor, toColor;
    public float duration = 1;
    public AnimationCurve onoffCurve;
    float current;
    public void On()
    {
        fromColor = emmisiveMaterial.GetColor("_EmissionColor");
        toColor = postHeatColor;
        current = 0;
    }
    public void Off()
    {
        fromColor = emmisiveMaterial.GetColor("_EmissionColor");
        toColor = preHeatColor;
        current = 0;
    }

    public void Update()
    {
        if (current >= duration)
        {
            current = duration;
            return;
        }
        emmisiveMaterial.SetColor("_EmissionColor", Color.Lerp(fromColor, toColor, onoffCurve.Evaluate((current += Time.deltaTime) / duration)));
    }

    public void OnApplicationQuit()
    {
        emmisiveMaterial.SetColor("_EmissionColor", preHeatColor);
    }
}
