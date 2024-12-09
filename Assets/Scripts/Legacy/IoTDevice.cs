using UnityEngine;

namespace IoTControl.Legacy
{
    public class IoTDevice : MonoBehaviour
    {
        public virtual IoTAction[] GetLabelledActionPairs() { return null; }
    }
}