using UnityEngine;

public class WindowIoTActionExecutor : MonoBehaviour
{
    public Transform LWindow, RWindow;
    Quaternion L_InitialRotation, R_InitialRotation;
    Quaternion L_StartRotation, R_StartRotation, L_EndRotation, R_EndRotation;
    public float maxOpenAmount = 140;
    public AnimationCurve opencloseCurve;
    public float duration = 1;
    float current = 1;

    private void Start()
    {
        L_InitialRotation = LWindow.localRotation;
        R_InitialRotation = RWindow.localRotation;
    }

    public void Open()
    {
        L_StartRotation = LWindow.localRotation;
        R_StartRotation = RWindow.localRotation;
        L_EndRotation = L_InitialRotation * Quaternion.Euler(0, maxOpenAmount, 0);
        R_EndRotation = R_InitialRotation * Quaternion.Euler(0, -maxOpenAmount, 0);
        current = 0;
    }
    public void Close()
    {
        L_StartRotation = LWindow.localRotation;
        R_StartRotation = RWindow.localRotation;
        L_EndRotation = L_InitialRotation;
        R_EndRotation = R_InitialRotation;
        current = 0;
    }

    public void Update()
    {
        if (current >= duration)
        {
            current = duration;
            return;
        }
        LWindow.localRotation = Quaternion.Lerp(L_StartRotation, L_EndRotation, opencloseCurve.Evaluate((current) / duration));
        RWindow.localRotation = Quaternion.Lerp(R_StartRotation, R_EndRotation, opencloseCurve.Evaluate((current += Time.deltaTime) / duration));
    }
}
