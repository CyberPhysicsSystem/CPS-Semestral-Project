using UnityEngine;

public class BulbIoTActionExecutor : MonoBehaviour
{
    public Material[] emmisiveMaterials;
    public Light pointlight;
    public void On()
    {
        foreach (Material mat in emmisiveMaterials)
            mat.EnableKeyword("_EMISSION");
        pointlight.enabled = true;
    }
    public void Off()
    {
        foreach (Material mat in emmisiveMaterials)
            mat.DisableKeyword("_EMISSION");
        pointlight.enabled = false;
    }

    public void OnApplicationQuit()
    {
        Off();
    }
}
