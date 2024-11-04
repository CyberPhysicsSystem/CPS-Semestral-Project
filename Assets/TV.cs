using System.Collections.Generic;
using UnityEngine;

public class TV : IoTDevice
{
    public List<IoTAction> myActions;

    public int volume, channel;
    public bool isOn;

    public Material screenMaterial;

    public bool debugPowerToggle;

    public void Awake()
    {
        myActions.Add(new IoTAction( label: "TurnOn", action: TurnOn ));
        myActions.Add(new IoTAction( label: "TurnOff", action: TurnOff ));
        myActions.Add(new IoTAction( label: "VolumeUp", action: VolumeUp ));
        myActions.Add(new IoTAction( label: "VolumeDown", action: VolumeDown ));
        myActions.Add(new IoTAction( label: "ChannelUp", action: ChannelUp ));
        myActions.Add(new IoTAction( label: "ChannelDown", action: ChannelDown));
    }
    public override IoTAction[] GetLabelledActionPairs() => myActions.ToArray();

    private void Update()
    {
        if(debugPowerToggle.Trigger())
        {
            if(isOn) TurnOff();
            else TurnOn();
        }
    }

    void TurnOn()
    {
        isOn = true;
        screenMaterial.SetInteger("IsOn", 1);
    }
    void TurnOff()
    {
        isOn = false;
        screenMaterial.SetInteger("IsOn", 0);
    }
    void VolumeUp() => volume += 1;
    void VolumeDown() => volume -= 1;
    void ChannelUp() => channel += 1;
    void ChannelDown() => channel -= 1;
}