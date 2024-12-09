using System;
using System.Collections.Generic;

namespace IoTControl.Legacy
{
    public class IoTActions
    {
        List<IoTAction> actions;
        List<string> action_labels;
        List<Action> action_actions;

        public IoTActions() : this(null, null, null) { }
        public IoTActions(List<IoTAction> actions, List<string> action_labels, List<Action> action_actions)
        {
            this.actions = actions;
            this.action_labels = action_labels;
            this.action_actions = action_actions;
        }
        public IoTActions(List<IoTAction> actions)
        {
            this.actions = actions;
            action_labels = new List<string>();
            action_actions = new List<Action>();
            foreach (var action in actions)
            {
                action_labels.Add(action.label);
                action_actions.Add(action.action);
            }
        }
    }
}