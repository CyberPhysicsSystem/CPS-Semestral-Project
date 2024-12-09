using System;

namespace IoTControl.Legacy
{
    public class IoTAction
    {
        public string label;
        public Action action;
        public IoTAction(string label, Action action)
        {
            this.label = label;
            this.action = action;
        }
    }
}