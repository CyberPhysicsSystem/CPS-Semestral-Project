using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class IoTDeviceHandler : MonoBehaviour
{
    public BaseIoTDevice[] devices;
    public int activeDevice;
    private int _activeDevice;
    public TMP_Dropdown dropdown;

    private void Awake()
    {
        devices = FindObjectsByType<BaseIoTDevice>(FindObjectsSortMode.None);
        UpdateDeviceStatus();
        dropdown.ClearOptions();
        foreach(var device in devices)
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text=device.gameObject.name });
    }

    private void OnValidate()
    {
        OnExternalChange();
    }

    void UpdateDeviceStatus()
    {
        for (int i = 0; i < devices.Length; ++i)
        {
            if (i == activeDevice) devices[i].SetActiveInteractionState(true);
            else devices[i].SetActiveInteractionState(false);
        }
    }

    public void OnDropdownValueChanged(int i)
    {
        activeDevice = i;
        OnExternalChange();
    }

    void OnExternalChange()
    {
        if (_activeDevice == activeDevice) return;
        _activeDevice = activeDevice;
        if (activeDevice >= devices.Length) activeDevice = devices.Length - 1;
        if (activeDevice < 0) activeDevice = 0;
        UpdateDeviceStatus();
    }

    public void SendActionToActiveDevice(string action)
    {
        Debug.Log("Selected Action was: " + action);
        if ((devices[activeDevice] as IIoTActionExecutor).TryDoAction(action))
            Debug.Log("Success");
        else
            Debug.Log("The action was not for this device");
    }
}
