using UnityEngine;

public class CookerIoTActionExecutor : MonoBehaviour
{
    public ParticleSystem[] mpss;
    public Light[] ls;

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
        foreach (ParticleSystem mps in mpss)
            mps.Play();
        foreach (Light pl in ls)
            pl.enabled = true;
    }
    public void Off()
    {
        foreach (ParticleSystem mps in mpss)
            mps.Stop();
        foreach (Light pl in ls)
            pl.enabled = false;
    }
}
