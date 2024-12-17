using UnityEngine;

public class TVIoTActionExecutor : MonoBehaviour
{
    public Material noiseableMaterial;
    public void On()
    {
        noiseableMaterial.SetFloat("_IsOn", 1);
    }
    public void Off()
    {
        noiseableMaterial.SetFloat("_IsOn", 0);
    }

    public void OnApplicationQuit()
    {
        Off();
    }
}
