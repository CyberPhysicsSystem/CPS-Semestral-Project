using UnityEngine;

public class ArcadeIoTActionExecutor : MonoBehaviour
{
    public Material noiseableMaterial;
    public Material emmisiveMaterial;

    public void On()
    {
        emmisiveMaterial.EnableKeyword("_EMISSION");
        noiseableMaterial.SetFloat("IsOn", 1);
    }
    public void Off()
    {
        emmisiveMaterial.DisableKeyword("_EMISSION");
        noiseableMaterial.SetFloat("IsOn", 0);
    }

    public void OnApplicationQuit()
    {
        Off();
    }
}
