using System.Collections.Generic;
using UnityEngine;

public class ActionLabelReceiver : TCPCommunicator
{
    public LabelMatcher matcher;
    float threshold = 1 / 15f; // because model's output_fps = 15,
    public override void MyUpdate()
    {
        if(ellapsed >= threshold)
        {
            ellapsed -= threshold;
            Send("Ready To Receive");
            string val = Receive();
            ActionPairs ap = JsonUtility.FromJson<ActionPairs>(val);
            matcher.RunRightExecutor(ap);
            // Debug.Log(val);
        }
        ellapsed += Time.deltaTime;
    }
}
