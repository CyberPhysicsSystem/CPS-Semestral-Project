using UnityEngine;
using System.Collections.Generic;

namespace IoTControl.Legacy
{
    public class IoTManager : MonoBehaviour
    {
        public IoTActions actions;

        void Start()
        {
            IoTDevice[] devices = FindObjectsByType<IoTDevice>(sortMode: FindObjectsSortMode.InstanceID);
            List<IoTAction> actions = new List<IoTAction>();
            foreach (var device in devices)
                actions.AddRange(device.GetLabelledActionPairs());
            this.actions = new IoTActions(actions);
        }
    }
}