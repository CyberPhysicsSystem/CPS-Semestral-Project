using UnityEngine;

public class LampIoTActionExecutor : MonoBehaviour
{
    public Material emmisiveMaterial;
    public Light pointlight;

    private void Awake()
    {
        Off();
    }

    public void OnApplicationQuit()
    {
        Off();
    }

    public void On()
    {
        emmisiveMaterial.EnableKeyword("_EMISSION");
        pointlight.enabled = true;
    }
    public void Off()
    {
        emmisiveMaterial.DisableKeyword("_EMISSION");
        pointlight.enabled = false;
    }
}
