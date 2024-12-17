using UnityEngine;

public class DummyQueryFeeder : MonoBehaviour
{
    public IoTGraphSearcher igs;
    public string dummyQuery;
    public bool trigger;

    private void Awake()
    {
        if (igs == null) igs = GetComponent<IoTGraphSearcher>();
    }

    private void Update()
    {
        if (trigger.Trigger())
            igs.Query(dummyQuery);
    }
}
