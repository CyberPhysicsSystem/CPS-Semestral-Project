using UnityEngine;

public class WasherIoTActionExecutor : MonoBehaviour
{
    public Animator mAnimator;
    public void On()
    {
        mAnimator.SetBool("On", true);
    }
    public void Off()
    {
        mAnimator.SetBool("On", false);
    }

    public void OnApplicationQuit()
    {
        Off();
    }
}
