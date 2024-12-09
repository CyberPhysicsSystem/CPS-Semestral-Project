using UnityEngine;

public class DayNightChanger : MonoBehaviour
{
    public Transform Sun;
    Quaternion startRotation, endRotation;
    public Vector3 dayRotation;
    public float duration = 1;
    public AnimationCurve daynightCurve;
    float current;
    private void Start()
    {
        current = duration;
    }
    public void TriggerDayNightShift()
    {
        startRotation = Sun.localRotation;
        endRotation = startRotation * Quaternion.Euler(0, 180, 0);
        current = 0;
    }
    void Update()
    {
        if (current >= duration)
        {
            current = duration;
            return;
        }
        Sun.localRotation = Quaternion.Lerp(startRotation, endRotation, daynightCurve.Evaluate((current += Time.deltaTime) / duration));
    }
}
