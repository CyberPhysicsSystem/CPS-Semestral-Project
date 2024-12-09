using UnityEngine;

public class DropdownManager : MonoBehaviour
{
    public IoTDeviceHandler idh;

    void Start()
    {
        if (!idh)
            idh = FindAnyObjectByType<IoTDeviceHandler>();
    }

    public void OnDropdownValueChanged(int i)
    {
        idh.OnDropdownValueChanged(i);
    }
}
