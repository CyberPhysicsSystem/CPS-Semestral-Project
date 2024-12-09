using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public interface IIoTActionProvider
{
    List<string> GetActionList();
}
public interface IIoTActionExecutor
{
    void DoAction(string actionName);
    bool TryDoAction(string actionName);
}
public interface IIoTDescriptor
{
    void SetActiveInteractionState(bool state);
    bool GetActiveInteractionState();
}

public class BaseIoTDevice : MonoBehaviour, IIoTActionProvider, IIoTActionExecutor, IIoTDescriptor
{
    public List<string> actionNames;
    public List<UnityEvent> actionEvents;
    public bool iAmActiveDevice;

    [SerializeField] private int debugInvokeIndex = 0;
    [SerializeField] private bool debugInvoke = false;

    /**
     * use if the action name is unsure for this IoT device
     */
    public bool TryDoAction(string actionName)
    {
        int temp = actionNames.IndexOf(actionName);
        if (temp == -1) return false;
        DoActionInternal(temp);
        return true;
    }

    /**
     * use only if the action name is for this IoT device
     */
    public void DoAction(string actionName) =>
        DoActionInternal(actionNames.IndexOf(actionName));
    private void DoActionInternal(int index) =>
        actionEvents[index]?.Invoke();

    public List<string> GetActionList()
    {
        return actionNames;
    }

    public bool GetActiveInteractionState()
    {
        return iAmActiveDevice;
    }

    public void SetActiveInteractionState(bool state)
    {
        iAmActiveDevice = state;
    }

    private void Update()
    {
        if (debugInvoke.Trigger())
        {
            actionEvents[debugInvokeIndex]?.Invoke();
        }
    }
}
