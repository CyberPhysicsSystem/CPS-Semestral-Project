using UnityEngine;
using TMPro;

public class MockupActionClassifier : MonoBehaviour
{
    public TMP_InputField inputField;
    //public IoTDeviceHandler IoTDeviceHandler;
    public IoTGraphSearcher igs;

    private void Start()
    {
        //if (!IoTDeviceHandler)
        //    IoTDeviceHandler = FindAnyObjectByType<IoTDeviceHandler>();
        if (!igs)
            igs = FindAnyObjectByType<IoTGraphSearcher>();
    }

    public void OnBatch()
    {
        var temp = inputField.text;
        if (!temp.Equals(""))
            //IoTDeviceHandler.SendActionToActiveDevice(temp);
            igs.Query(temp);
    }
}
