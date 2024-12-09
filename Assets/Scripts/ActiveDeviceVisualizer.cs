using UnityEngine;

public class ActiveDeviceVisualizer : MonoBehaviour
{
    public BaseIoTDevice IoTDeviceToLook;
    bool prevState;
    public GameObject VisualCompoment;

    void Start()
    {
        IoTDeviceToLook = GetComponent<BaseIoTDevice>();
        VisualCompoment.SetActive(prevState = IoTDeviceToLook.GetActiveInteractionState());
    }

    void Update()
    {
        if(prevState != IoTDeviceToLook.GetActiveInteractionState())
            VisualCompoment.SetActive(prevState = !prevState);
    }
}
