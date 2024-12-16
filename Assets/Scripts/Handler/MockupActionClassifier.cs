using UnityEngine;
using TMPro;

public class MockupActionClassifier : MonoBehaviour
{
    public TMP_InputField inputField;
    public IoTDeviceHandler IoTDeviceHandler;

    private void Start()
    {
        if (!IoTDeviceHandler)
            IoTDeviceHandler = FindAnyObjectByType<IoTDeviceHandler>();
    }

    public void OnBatch()
    {
        var temp = inputField.text;
        if (!temp.Equals(""))
            IoTDeviceHandler.SendActionToActiveDevice(temp);
    }
}
