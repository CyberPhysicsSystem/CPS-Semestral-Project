using System.Collections.Generic;

[System.Serializable]
public class ActionPair
{
    public string key;
    public float value;
}

[System.Serializable]
public class ActionPairs
{
    public List<ActionPair> pairs;
}